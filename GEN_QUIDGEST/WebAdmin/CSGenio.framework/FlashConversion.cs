using System;
using System.Collections.Generic;
using System.Text;

namespace CSGenio.framework
{
    public static class FlashConversion
    {
        /// <summary>
        /// Converte um objecto de flash to string
        /// </summary>
        /// <param name="valor">O objecto de flash</param>
        /// <returns>A objecto convertido to o tipo interno</returns>
        public static string ToString(object Qvalue)
        {
            if (Qvalue == null)
                return "";
            return Qvalue.ToString();
        }

        /// <summary>
        /// Converte uma string to um objecto de flash
        /// </summary>
        /// <param name="valor">O Qvalue interno</param>
        /// <returns>O Qvalue interno convertido to flash</returns>
        public static string FromString(string Qvalue)
        {
            if (Qvalue == null)
               return "";
            return Qvalue;
        }

        /// <summary>
        /// Converte um objecto de flash to numérico
        /// </summary>
        /// <param name="valor">O objecto de flash</param>
        /// <returns>A objecto convertido to o tipo interno</returns>
        public static decimal ToNumeric(object Qvalue)
        {
            if (Qvalue == null)
                return 0;
            if (Qvalue is string)
            {
                if (Qvalue.Equals(""))
                    return 0;

                decimal temp = 0;
                if (!decimal.TryParse(Qvalue.ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.CurrentCulture, out temp) &&
                    !decimal.TryParse(Qvalue.ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out temp))
                    return 0;
                else
                    return temp;
            }

            return 0;
        }

        /// <summary>
        /// Converte um numérico to um objecto de flash
        /// </summary>
        /// <param name="valor">O Qvalue interno</param>
        /// <returns>O Qvalue interno convertido to flash</returns>
        public static string FromNumeric(decimal Qvalue)
        {
            return Qvalue.ToString().Replace(',', '.');
        }

        /// <summary>
        /// Converte um objecto de flash to inteiro
        /// </summary>
        /// <param name="valor">O objecto de flash</param>
        /// <returns>A objecto convertido to o tipo interno</returns>
        public static int ToInteger(object Qvalue)
        {
            try
            {
                return Convert.ToInt32(Qvalue);
            }
            catch 
			{ 
				return 0;
			}
        }

        /// <summary>
        /// Converte um inteiro to um objecto de flash
        /// </summary>
        /// <param name="valor">O Qvalue interno</param>
        /// <returns>O Qvalue interno convertido to flash</returns>
        public static string FromInteger(int Qvalue)
        {
            return Convert.ToString(Qvalue);
        }

        /// <summary>
        /// Converte um objecto de flash to uma data
        /// </summary>
        /// <param name="valor">O objecto de flash</param>
        /// <returns>A objecto convertido to o tipo interno</returns>
        public static DateTime ToDateTime(object Qvalue)
        {
            if (Qvalue == null)
                return DateTime.MinValue;
            else
                return Convert.ToDateTime(Qvalue);
        }

        /// <summary>
        /// Converte uma data to um objecto de flash
        /// </summary>
        /// <param name="valor">O Qvalue interno</param>
        /// <returns>O Qvalue interno convertido to flash</returns>
        public static string FromDateTime(DateTime Qvalue, bool hasTime, bool hasSeconds)
        {
            try
            {
                if (Qvalue == DateTime.MinValue)
                    return "";
                StringBuilder sb = new StringBuilder();
               

                //Assume que a data vai ser fornecida ao Qweb e portanto deve ser sempre entregue no format Dia/month/Qyear
                sb.Append( Qvalue.Year.ToString().PadLeft(4, '0') + "/" );
                sb.Append( Qvalue.Month.ToString().PadLeft(2, '0') + "/" );
                sb.Append( Qvalue.Day.ToString().PadLeft(2, '0') );

                if (hasTime)
                {
                    sb.Append( " " + Qvalue.Hour.ToString().PadLeft(2, '0') + ":" );
                    sb.Append( Qvalue.Minute.ToString().PadLeft(2, '0') );
                    if (hasSeconds)
                        sb.Append( ":" + Qvalue.Second.ToString().PadLeft(2, '0') );
                }

                return sb.ToString();
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Converte um objecto de flash to um lógico
        /// </summary>
        /// <param name="valor">O objecto de flash</param>
        /// <returns>A objecto convertido to o tipo interno</returns>
        public static int ToLogic(object Qvalue)
        {
            if (Qvalue == null)
                return 0;

            if (Qvalue is string)
            {
                if (Qvalue.Equals(""))
                    return 0;
                else
                {
                    int temp = 0;
                    if (int.TryParse(Qvalue.ToString(), out temp))
                        return temp;
                    else
                        return 0;
                }
            }
            else if (Qvalue is bool)
            {
                return ((bool)Qvalue) ? 1 : 0;
            }
            return 0;
        }

        /// <summary>
        /// Converte um lógico to um objecto de flash
        /// </summary>
        /// <param name="valor">O Qvalue interno</param>
        /// <returns>O Qvalue interno convertido to flash</returns>
        public static string FromLogic(int Qvalue)
        {
            return Qvalue.ToString();
        }

        /// <summary>
        /// Converte um objecto de flash to uma key primária ou estrangeira
        /// </summary>
        /// <param name="valor">O objecto de flash</param>
        /// <returns>A objecto convertido to o tipo interno</returns>
        public static string ToKey(object Qvalue)
        {
            if (Qvalue == null || Qvalue.ToString() == "00000000-0000-0000-0000-000000000000")
                return "";
            else
                return Qvalue.ToString();
        }

        /// <summary>
        /// Converte uma key primária ou estrangeira to um objecto de flash
        /// </summary>
        /// <param name="valor">O Qvalue interno</param>
        /// <returns>O Qvalue interno convertido to flash</returns>
        public static string FromKey(string Qvalue)
        {
            string Qresult = "";
            if (Qvalue != null)
            {
                Qresult = Qvalue;
                if (Qvalue.Length == 0 || Qvalue == "00000000-0000-0000-0000-000000000000")
                    Qresult = "";
            }
            return Qresult;
        }

        /// <summary>
        /// Converte um objecto de flash to um binário
        /// </summary>
        /// <param name="valor">O objecto de flash</param>
        /// <returns>A objecto convertido to o tipo interno</returns>
        public static byte[] ToBinary(object Qvalue)
        {
            if (Qvalue == null || Qvalue.Equals(""))
                return new Byte[0];
            else if (Qvalue is string)
            {
                string s = Qvalue as string;
                byte[] bytes = new byte[s.Length / 2];
                for (int i = 0; i < s.Length / 2; i += 2)
                    bytes[i / 2] = Convert.ToByte(s, 16);
                return bytes;
            }

            return new Byte[0];
        }

        /// <summary>
        /// Converte uma binário to um objecto de flash
        /// </summary>
        /// <param name="valor">O Qvalue interno</param>
        /// <returns>O Qvalue interno convertido to flash</returns>
        public static string FromBinary(byte[] Qvalue)
        {
            if (Qvalue == null)
                return "";
            else
            {
                Byte[] file = (Byte[])Qvalue;
                string ficheiroString = "";
                ficheiroString = BitConverter.ToString(file).Replace("-", string.Empty);
                return ficheiroString;
            }
        }
		
        // (RS 2011.01.20) From acordo com http://stackoverflow.com/questions/311165/how-do-you-convert-byte-array-to-hexadecimal-string-and-vice-versa-in-c
        // esta conversão de byte array to string hexadecimal é bastante mais eficiente
        // TODO: testar e usar
        /*
        private static string ByteArrayToHex(byte[] barray) 
        {
            char[] c = new char[barray.Length * 2];
            byte b;
            for (int i = 0; i < barray.Length; ++i)
            {
                b = ((byte)(barray[i] >> 4));
                c[i * 2] = (char)(b > 9 ? b + 0x37 : b + 0x30);
                b = ((byte)(barray[i] & 0xF));
                c[i * 2 + 1] = (char)(b > 9 ? b + 0x37 : b + 0x30);
            }
            return new string(c); 
        }
        */

        /// <summary>
        /// Converte um objecto de flash to um objecto interno
        /// </summary>
        /// <param name="valor">O objecto de flash</param>
        /// <returns>A objecto convertido to o tipo interno</returns>
        public static object ToInternal(object Qvalue, FieldFormatting formatting)
        {
            switch (formatting)
            {
                case FieldFormatting.INTEIRO:
                case FieldFormatting.LOGICO:
                    return ToLogic(Qvalue);
                case FieldFormatting.FLOAT:
                    return ToNumeric(Qvalue);
                case FieldFormatting.DATA:
                case FieldFormatting.DATAHORA:
                case FieldFormatting.DATASEGUNDO:
                    return ToDateTime(Qvalue);
                case FieldFormatting.TEMPO:
                case FieldFormatting.CARACTERES:
                    return ToString(Qvalue);
                case FieldFormatting.GUID:
                    return ToKey(Qvalue);
                case FieldFormatting.JPEG:
                case FieldFormatting.BINARIO:
                    return ToBinary(Qvalue);
            }
            
			throw new FrameworkException(null, "FlashConversion.ToInterno", "Format not recognized: " + formatting.ToString());
        }

        /// <summary>
        /// Converte um objecto interno to um objecto de flash
        /// </summary>
        /// <param name="valor">O Qvalue interno</param>
        /// <returns>O Qvalue interno convertido to flash</returns>
        public static string FromInternal(object Qvalue, FieldFormatting formatting)
        {
            try
            {
                switch (formatting)
                {
                    case FieldFormatting.TEMPO:
                    case FieldFormatting.CARACTERES:
                        return FromString(Qvalue as string);
                    case FieldFormatting.GUID:
                        return FromKey(Qvalue as string);
                    case FieldFormatting.INTEIRO:
                    case FieldFormatting.LOGICO:
                        return FromLogic((int)Qvalue);
                    case FieldFormatting.FLOAT:
                        return FromNumeric((decimal)Qvalue);
                    case FieldFormatting.DATA:
                        return FromDateTime((DateTime)Qvalue, false, false);
                    case FieldFormatting.DATAHORA:
                        return FromDateTime((DateTime)Qvalue, true, false);
                    case FieldFormatting.DATASEGUNDO:
                        return FromDateTime((DateTime)Qvalue, true, true);
                    case FieldFormatting.JPEG:
                    case FieldFormatting.BINARIO:
                        return FromBinary(Qvalue as byte[]);
                    default:
                        throw new FrameworkException(null, "FlashConversion.FromInterno", "Format not recognized: " + formatting.ToString());
                }
            }
			catch (GenioException ex)
			{
				if (ex.ExceptionSite == "FlashConversion.FromInterno")
					throw;
				throw new FrameworkException(ex.UserMessage, "FlashConversion.FromInterno", "Arror converting internal type to string: " + ex.Message, ex);
			}
            catch(Exception ex) //os cast podem falhar
            {
                throw new FrameworkException(null, "FlashConversion.FromInterno", "Arror converting internal type to string: " + ex.Message, ex);
            }
        }
    }
}
