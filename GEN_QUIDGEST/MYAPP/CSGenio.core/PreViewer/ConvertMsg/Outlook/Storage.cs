﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using STATSTG = System.Runtime.InteropServices.ComTypes.STATSTG;

namespace GenioServer.PreViewer.ConvertMsg.Outlook
{
    public partial class Storage : IDisposable
    {
        #region Fields
        /// <summary>
        /// The statistics for all streams in the IStorage associated with this instance
        /// </summary>
        private readonly Dictionary<string, STATSTG> _streamStatistics = new Dictionary<string, STATSTG>();

        /// <summary>
        /// The statistics for all storgages in the IStorage associated with this instance
        /// </summary>
        private readonly Dictionary<string, STATSTG> _subStorageStatistics = new Dictionary<string, STATSTG>();

        /// <summary>
        /// Header size of the property stream in the IStorage associated with this instance
        /// </summary>
        private int _propHeaderSize = MapiTags.PropertiesStreamHeaderTop;

        /// <summary>
        /// A reference to the parent message that this message may belong to
        /// </summary>
        private Storage _parentMessage;

        /// <summary>
        /// The IStorage associated with this instance.
        /// </summary>
        private NativeMethods.IStorage _storage;

        /// <summary>
        /// Will contain all the named MAPI properties when the class that inherits the <see cref="Storage"/> class 
        /// is a <see cref="Storage.Message"/> class. Otherwhise the List will be null
        /// mapped to
        /// </summary>
        private List<MapiTagMapping> _namedProperties; 
        #endregion

        #region Properties
        /// <summary>
        /// Gets the top level outlook message from a sub message at any level.
        /// </summary>
        /// <value> The top level outlook message. </value>
        private Storage TopParent
        {
            get { return _parentMessage != null ? _parentMessage.TopParent : this; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is the top level outlook message.
        /// </summary>
        /// <value> <c>true</c> if this instance is the top level outlook message; otherwise, <c>false</c> . </value>
        private bool IsTopParent
        {
            get { return _parentMessage == null; }
        }
        #endregion

        #region Constructors & Destructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Storage" /> class from a file.
        /// </summary>
        /// <param name="storageFilePath"> The file to load. </param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        private Storage(string storageFilePath)
        {
            // Ensure provided file is an IStorage
            if (NativeMethods.StgIsStorageFile(storageFilePath) != 0)
                throw new ArgumentException("The provided file is not a valid IStorage", "storageFilePath");

            // Open and load IStorage from file
            NativeMethods.IStorage fileStorage;
            NativeMethods.StgOpenStorage(storageFilePath, null, NativeMethods.Stgm.Read | NativeMethods.Stgm.ShareDenyWrite, IntPtr.Zero, 0, out fileStorage);
            
            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            LoadStorage(fileStorage);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Storage" /> class from a <see cref="Stream" /> containing an IStorage.
        /// </summary>
        /// <param name="storageStream"> The <see cref="Stream" /> containing an IStorage. </param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        private Storage(Stream storageStream)
        {
            NativeMethods.IStorage memoryStorage = null;
            NativeMethods.ILockBytes memoryStorageBytes = null;
            try
            {
                // Read stream into buffer
                var buffer = new byte[storageStream.Length];
                storageStream.Read(buffer, 0, buffer.Length);

                // Create a ILockBytes (unmanaged byte array) and write buffer into it
                NativeMethods.CreateILockBytesOnHGlobal(IntPtr.Zero, true, out memoryStorageBytes);
                memoryStorageBytes.WriteAt(0, buffer, buffer.Length, null);

                // Ensure provided stream data is an IStorage
                if (NativeMethods.StgIsStorageILockBytes(memoryStorageBytes) != 0)
                    throw new ArgumentException("The provided stream is not a valid IStorage", "storageStream");

                // Open and load IStorage on the ILockBytes
                NativeMethods.StgOpenStorageOnILockBytes(memoryStorageBytes, null, NativeMethods.Stgm.Read | NativeMethods.Stgm.ShareDenyWrite, IntPtr.Zero, 0, out memoryStorage);
                
                // ReSharper disable once DoNotCallOverridableMethodsInConstructor
                LoadStorage(memoryStorage);
            }
            catch
            {
                if (memoryStorage != null)
                    Marshal.ReleaseComObject(memoryStorage);
            }
            finally
            {
                if (memoryStorageBytes != null)
                    Marshal.ReleaseComObject(memoryStorageBytes);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Storage" /> class on the specified <see cref="NativeMethods.IStorage" />.
        /// </summary>
        /// <param name="storage"> The storage to create the <see cref="Storage" /> on. </param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        private Storage(NativeMethods.IStorage storage)
        {
            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            LoadStorage(storage);
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="Storage" /> is reclaimed by garbage collection.
        /// </summary>
        ~Storage()
        {
            Dispose(false);
        }
        #endregion

        #region LoadStorage
        /// <summary>
        /// Processes sub streams and storages on the specified storage.
        /// </summary>
        /// <param name="storage"> The storage to get sub streams and storages for. </param>
        protected virtual void LoadStorage(NativeMethods.IStorage storage)
        {
            _storage = storage;

             // Ensures memory is released
            ReferenceManager.AddItem(storage);
            NativeMethods.IEnumSTATSTG storageElementEnum = null;

            try
            {
                // Enum all elements of the storage
                storage.EnumElements(0, IntPtr.Zero, 0, out storageElementEnum);

                // Iterate elements
                while (true)
                {
                    // Get 1 element out of the com enumerator
                    uint elementStatCount;
                    var elementStats = new STATSTG[1];
                    storageElementEnum.Next(1, elementStats, out elementStatCount);

                    // Break loop if element not retrieved
                    if (elementStatCount != 1)
                        break;

                    var elementStat = elementStats[0];
                    switch (elementStat.type)
                    {
                        case 1:
                            // Element is a storage. add its statistics object to the storage dictionary
                            _subStorageStatistics.Add(elementStat.pwcsName, elementStat);
                            break;

                        case 2:
                            // Element is a stream. add its statistics object to the stream dictionary
                            _streamStatistics.Add(elementStat.pwcsName, elementStat);
                            break;
                    }
                }
            }
            finally
            {
                // Free memory
                if (storageElementEnum != null)
                    Marshal.ReleaseComObject(storageElementEnum);
            }
        }
        #endregion

        #region GetStreamBytes
        /// <summary>
        /// Gets the data in the specified stream as a byte array.
        /// </summary>
        /// <param name="streamName"> Name of the stream to get data for. </param>
        /// <returns> A byte array containg the stream data. </returns>
        private byte[] GetStreamBytes(string streamName)
        {
            // Get statistics for stream 
            var streamStatStg = _streamStatistics[streamName];

            byte[] iStreamContent;
            IStream stream = null;
            try
            {
                // Open stream from the storage
                stream = _storage.OpenStream(streamStatStg.pwcsName, IntPtr.Zero,
                    NativeMethods.Stgm.Read | NativeMethods.Stgm.ShareExclusive, 0);

                // Read the stream into a managed byte array
                iStreamContent = new byte[streamStatStg.cbSize];
                stream.Read(iStreamContent, iStreamContent.Length, IntPtr.Zero);
            }
            finally
            {
                if (stream != null)
                    Marshal.ReleaseComObject(stream);
            }

            // Return the stream bytes
            return iStreamContent;
        }
        #endregion

        #region GetStreamAsString
        /// <summary>
        /// Gets the data in the specified stream as a string using the specifed encoding to decode the stream data.
        /// </summary>
        /// <param name="streamName"> Name of the stream to get string data for. </param>
        /// <param name="streamEncoding"> The encoding to decode the stream data with. </param>
        /// <returns> The data in the specified stream as a string. </returns>
        private string GetStreamAsString(string streamName, Encoding streamEncoding)
        {
            var streamReader = new StreamReader(new MemoryStream(GetStreamBytes(streamName)), streamEncoding);
            var streamContent = streamReader.ReadToEnd();
            streamReader.Close();

            // Remove null termination chars when they exist
            return streamContent.Replace("\0", string.Empty);
        }
        #endregion

        #region GetMapiProperty
        /// <summary>
        /// Gets the raw value of the MAPI property.
        /// </summary>
        /// <param name="propIdentifier"> The 4 char hexadecimal prop identifier. </param>
        /// <returns> The raw value of the MAPI property. </returns>
        private object GetMapiProperty(string propIdentifier)
        {
            // Check if the propIdentifier is a named property and if so replace it with
            // the correct mapped property
            if (_namedProperties != null)
            {
                var mapiTagMapping = _namedProperties.Find(m => m.EntryOrStringIdentifier == propIdentifier);
                if (mapiTagMapping != null)
                    propIdentifier = mapiTagMapping.PropertyIdentifier;
            }

            // Try get prop value from stream or storage
            // If not found in stream or storage try get prop value from property stream
            var propValue = GetMapiPropertyFromStreamOrStorage(propIdentifier) ??
                            GetMapiPropertyFromPropertyStream(propIdentifier);

            
            return propValue;
        }
        #endregion

        #region GetMapiPropertyFromStreamOrStorage
        /// <summary>
        /// Gets the MAPI property value from a stream or storage in this storage.
        /// </summary>
        /// <param name="propIdentifier"> The 4 char hexadecimal prop identifier. </param>
        /// <returns> The value of the MAPI property or null if not found. </returns>
        private object GetMapiPropertyFromStreamOrStorage(string propIdentifier)
        {
            // Get list of stream and storage identifiers which map to properties
            var propKeys = new List<string>();
            propKeys.AddRange(_streamStatistics.Keys);
            propKeys.AddRange(_subStorageStatistics.Keys);

            // Determine if the property identifier is in a stream or sub storage
            string propTag = null;
            var propType = MapiTags.PT_UNSPECIFIED;

            foreach (var propKey in propKeys)
            {
                if (!propKey.StartsWith(MapiTags.SubStgVersion1 + "_" + propIdentifier)) continue;
                propTag = propKey.Substring(12, 8);
                propType = ushort.Parse(propKey.Substring(16, 4), NumberStyles.HexNumber);
                break;
            }

            // When null then we didn't find the property
            if (propTag == null)
                return null;

            // Depending on prop type use method to get property value
            var containerName = MapiTags.SubStgVersion1 + "_" + propTag;
            switch (propType)
            {
                case MapiTags.PT_UNSPECIFIED:
                    return null;

                case MapiTags.PT_STRING8:
                    //return GetStreamAsString(containerName, Encoding.UTF8);
                    return GetStreamAsString(containerName, Encoding.Default);

                case MapiTags.PT_UNICODE:
                    return GetStreamAsString(containerName, Encoding.Unicode);

                case MapiTags.PT_BINARY:
                    return GetStreamBytes(containerName);

                case MapiTags.PT_MV_UNICODE:

                    // If the property is a unicode multiview item we need to read all the properties
                    // again and filter out all the multivalue names, they end with -00000000, -00000001, etc..
                    var multiValueContainerNames = propKeys.Where(propKey => propKey.StartsWith(containerName + "-")).ToList();

                    var values = new List<string>();
                    foreach (var multiValueContainerName in multiValueContainerNames)
                    {
                        var value = GetStreamAsString(multiValueContainerName, Encoding.Unicode);
                        // multi values always end with a null char so we need to strip that one off
                        if (value.EndsWith("/0"))
                            value = value.Substring(0, value.Length - 1);

                        values.Add(value);
                    }
                    
                    return values;
                    
                case MapiTags.PT_OBJECT:
                    return
                        NativeMethods.CloneStorage(
                            _storage.OpenStorage(containerName, IntPtr.Zero,
                                NativeMethods.Stgm.Read | NativeMethods.Stgm.ShareExclusive,
                                IntPtr.Zero, 0), true);

                default:
                    {
                        return null;
                        //throw new ApplicationException("MAPI property has an unsupported type and can not be retrieved.");
                    }
            }
        }

        /// <summary>
        /// Gets the MAPI property value from the property stream in this storage.
        /// </summary>
        /// <param name="propIdentifier"> The 4 char hexadecimal prop identifier. </param>
        /// <returns> The value of the MAPI property or null if not found. </returns>
        private object GetMapiPropertyFromPropertyStream(string propIdentifier)
        {
            // If no property stream return null
            if (!_streamStatistics.ContainsKey(MapiTags.PropertiesStream))
                return null;

            // Get the raw bytes for the property stream
            var propBytes = GetStreamBytes(MapiTags.PropertiesStream);

            // Iterate over property stream in 16 byte chunks starting from end of header
            for (var i = _propHeaderSize; i < propBytes.Length; i = i + 16)
            {
                // Get property type located in the 1st and 2nd bytes as a unsigned short value
                var propType = BitConverter.ToUInt16(propBytes, i);

                // Get property identifer located in 3nd and 4th bytes as a hexdecimal string
                var propIdent = new[] { propBytes[i + 3], propBytes[i + 2] };
                var propIdentString = BitConverter.ToString(propIdent).Replace("-", string.Empty);

                // If this is not the property being gotten continue to next property
                if (propIdentString != propIdentifier) continue;

                // Depending on prop type use method to get property value
                switch (propType)
                {
                    case MapiTags.PT_I2:
                        return BitConverter.ToInt16(propBytes, i + 8);

                    case MapiTags.PT_LONG:
                        return BitConverter.ToInt32(propBytes, i + 8);

                    case MapiTags.PT_DOUBLE:
                        return BitConverter.ToDouble(propBytes, i + 8);

                    case MapiTags.PT_SYSTIME:
                        var fileTime = BitConverter.ToInt64(propBytes, i + 8);
                        return DateTime.FromFileTime(fileTime);

                    case MapiTags.PT_BOOLEAN:
                        return BitConverter.ToBoolean(propBytes, i + 8);

                    //default:
                    //throw new ApplicationException("MAPI property has an unsupported type and can not be retrieved.");
                }
            }

            // Property not found return null
            return null;
        }

        /// <summary>
        /// Gets the value of the MAPI property as a string.
        /// </summary>
        /// <param name="propIdentifier"> The 4 char hexadecimal prop identifier. </param>
        /// <returns> The value of the MAPI property as a string. </returns>
        private string GetMapiPropertyString(string propIdentifier)
        {
            return GetMapiProperty(propIdentifier) as string;
        }

        /// <summary>
        /// Gets the value of the MAPI property as a list of string.
        /// </summary>
        /// <param name="propIdentifier"> The 4 char hexadecimal prop identifier. </param>
        /// <returns> The value of the MAPI property as a list of string. </returns>
        private ReadOnlyCollection<string> GetMapiPropertyStringList(string propIdentifier)
        {
            var list = GetMapiProperty(propIdentifier) as List<string>;
            return list == null ? null : list.AsReadOnly();
        }

        /// <summary>
        /// Gets the value of the MAPI property as a integer.
        /// </summary>
        /// <param name="propIdentifier"> The 4 char hexadecimal prop identifier. </param>
        /// <returns> The value of the MAPI property as a integer. </returns>
        private int? GetMapiPropertyInt32(string propIdentifier)
        {
            var value = GetMapiProperty(propIdentifier);

            if (value != null)
                return (int) value;

            return null;
        }

        /// <summary>
        /// Gets the value of the MAPI property as a double.
        /// </summary>
        /// <param name="propIdentifier"> The 4 char hexadecimal prop identifier. </param>
        /// <returns> The value of the MAPI property as a double. </returns>
        private double? GetMapiPropertyDouble(string propIdentifier)
        {
            var value = GetMapiProperty(propIdentifier);

            if (value != null)
                return (double) value;

            return null;
        }

        /// <summary>
        /// Gets the value of the MAPI property as a datetime.
        /// </summary>
        /// <param name="propIdentifier"> The 4 char hexadecimal prop identifier. </param>
        /// <returns> The value of the MAPI property as a datetime or null when not set </returns>
        private DateTime? GetMapiPropertyDateTime(string propIdentifier)
        {
            var value = GetMapiProperty(propIdentifier);

            if (value != null)
                return (DateTime) value;

            return null;
        }

        /// <summary>
        /// Gets the value of the MAPI property as a bool.
        /// </summary>
        /// <param name="propIdentifier"> The 4 char hexadecimal prop identifier. </param>
        /// <returns> The value of the MAPI property as a boolean or null when not set. </returns>
        private bool? GetMapiPropertyBool(string propIdentifier)
        {
            var value = GetMapiProperty(propIdentifier);

            if (value != null)
                return (bool) value;

            return null;
        }

        /// <summary>
        /// Gets the value of the MAPI property as a byte array.
        /// </summary>
        /// <param name="propIdentifier"> The 4 char hexadecimal prop identifier. </param>
        /// <returns> The value of the MAPI property as a byte array. </returns>
        private byte[] GetMapiPropertyBytes(string propIdentifier)
        {
            return (byte[]) GetMapiProperty(propIdentifier);
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                Disposing();

            if (_storage == null) return;
            ReferenceManager.RemoveItem(_storage);
            Marshal.ReleaseComObject(_storage);
            _storage = null;
        }

        /// <summary>
        /// Gives sub classes the chance to free resources during object disposal.
        /// </summary>
        protected virtual void Disposing()
        {
        }
        #endregion
    }
}