using NUnit.Framework;

using CSGenio.framework;

namespace WebTest
{

    /// <summary>
    ///This is a test class for Test and is intended
    ///to contain all Test Unit Tests
    ///</summary>
    public class TestConversaoCrystal
    {

        /// <summary>
        /// Teste aos metodos de To(xxx) que devem retornar uma excepção de operação invalida
        /// </summary>
        [Test]
        public void TestToMethods()
        {
            Assert.Throws<FrameworkException>(() =>
            {
                CrystalConversion.ToString(null);
            });
            Assert.Throws<FrameworkException>(() =>
            {
                CrystalConversion.ToInteger(null);
            });
            Assert.Throws<FrameworkException>(() =>
            {
                CrystalConversion.ToNumeric(null);
            });
            Assert.Throws<FrameworkException>(() =>
            {
                CrystalConversion.ToDateTime(null);
            });
            Assert.Throws<FrameworkException>(() =>
            {
                CrystalConversion.ToKey(null);
            });
            Assert.Throws<FrameworkException>(() =>
            {
                CrystalConversion.ToLogic(null);
            });
            Assert.Throws<FrameworkException>(() =>
            {
                CrystalConversion.ToBinary(null);
            });
            Assert.Throws<FrameworkException>(() =>
            {
                CrystalConversion.ToInternal(null, FieldFormatting.CARACTERES);
            });
        }

        /// <summary>
        /// Teste à função FromString
        /// </summary>
        [Test]
        public void TestFromString()
        {
            string res = null;

            // null --> empty string
            res = CrystalConversion.FromString(null);
            // Até mais ver, Qvalues "vazios" são representados por strings de comprimento zero, e não por NULL
            Assert.AreEqual("\"\"", res);

            // empty string --> empty string
            res = CrystalConversion.FromString("");
            // Até mais ver, Qvalues "vazios" são representados por strings de comprimento zero, e não por NULL
            Assert.AreEqual("\"\"", res);

            // single space --> single space
            res = CrystalConversion.FromString(" ");
            Assert.AreEqual("\" \"", res);

            // string with a number --> string with a number
            res = CrystalConversion.FromString("134");
            Assert.AreEqual("\"134\"", res);

            // bunch of Unicode characters --> same bunch of Unicode characters
            res = CrystalConversion.FromString("¡¢£¤¥¦§¨©ª«¬®¯°±²³´¶·¸¹º»¼½¾¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷øùúûüýþÿ");
            Assert.AreEqual("\"¡¢£¤¥¦§¨©ª«¬®¯°±²³´¶·¸¹º»¼½¾¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ÷ØÙÚÛÜÝÞŸ\"", res);
        }

        /// <summary>
        /// Teste à função FromInteger
        /// </summary>
        [Test]
        public void TestFromInteger()
        {
            // Como CrystalConversion.FromInteger é essencialmente um Convert.ToString(int),
            // este test é algo ingrato

            string res = null;

            res = CrystalConversion.FromInteger(0);
            Assert.AreEqual("0", res);

            res = CrystalConversion.FromInteger(-2);
            Assert.AreEqual("-2", res);

            res = CrystalConversion.FromInteger(9000);
            Assert.AreEqual("9000", res);
        }

        /// <summary>
        ///Teste à função FromDateTime
        /// </summary>
        [Test]
        public void TestFromDateTime()
        {
            string res = null;

            res = CrystalConversion.FromDateTime(DateTime.MinValue, true, true);
            Assert.AreEqual("", res);

            res = CrystalConversion.FromDateTime(new DateTime(1900, 2, 1), true, true);
            Assert.AreEqual("DateTime(1900, 02, 01, 00, 00, 00)", res);
        }

        /// <summary>
        /// Teste à função FromLogic
        /// </summary>
        [Test]
        public void TestFromLogic()
        {
            string res = null;

            res = CrystalConversion.FromLogic(int.MinValue);
            //Assert.AreEqual(?, res);

            res = CrystalConversion.FromLogic(-1);
            Assert.AreEqual("-1", res); // Ou erro?

            res = CrystalConversion.FromLogic(0);
            Assert.AreEqual("0", res);

            res = CrystalConversion.FromLogic(1);
            Assert.AreEqual("1", res);

            res = CrystalConversion.FromLogic(int.MaxValue);
            //Assert.AreEqual(?, res);
        }

        /// <summary>
        /// Teste à função FromKey
        /// </summary>
        [Test]
        public void TestFromKey()
        {
            string res = null;

            // empty string --> NULL
            res = CrystalConversion.FromKey(string.Empty);
            Assert.AreEqual("\"{00000000-0000-0000-0000-000000000000}\"", res);

            // null --> NULL
            res = CrystalConversion.FromKey(null);
            Assert.AreEqual("\"{00000000-0000-0000-0000-000000000000}\"", res);

            // empty GUID (as string) --> NULL
            res = CrystalConversion.FromKey(Guid.Empty.ToString());
            Assert.AreEqual("\"{00000000-0000-0000-0000-000000000000}\"", res);

            // zeroed GUID (no quotes) --> NULL
            res = CrystalConversion.FromKey("00000000-0000-0000-0000-000000000000");
            Assert.AreEqual("\"{00000000-0000-0000-0000-000000000000}\"", res);

            // non-empty GUID (no quotes) --> same GUID (quoted)
            res = CrystalConversion.FromKey("a6bb0086-8c37-4b88-8235-161f76134a2a");
            Assert.AreEqual("\"{A6BB0086-8C37-4B88-8235-161F76134A2A}\"", res);
        }

        /// <summary>
        /// Teste à função FromBinary
        /// </summary>
        [Test]
        public void TestFromBinary()
        {
        }

        /// <summary>
        /// Teste à função FromInternal
        /// </summary>
        [Test]
        public void TestFromInterno()
        {
            string res = null;

            // var value in System.Enum.GetValues(typeof(FieldFormatting)) ){}
            /*
            FieldFormatting.ANO_MES_DIA;
            FieldFormatting.BINARIO;
            FieldFormatting.CARACTERES;
            FieldFormatting.DATA;
            FieldFormatting.DATAHORA;
            FieldFormatting.DATASEGUNDO;
            FieldFormatting.DIA_MES_ANO;
            FieldFormatting.FLOAT;
            FieldFormatting.GUID;
            FieldFormatting.INTEIRO;
            FieldFormatting.JPEG;
            FieldFormatting.LOGICO;
            */

            //
            // FieldFormatting.ANO_MES_DIA
            //

            // Não é usado

            //
            // FieldFormatting.BINARIO
            //
            /*
            res = CrystalConversion.FromInternal(null, FieldFormatting.BINARIO);
            Assert.AreEqual("NULL", res);

            res = CrystalConversion.FromInternal("", FieldFormatting.BINARIO);
            Assert.AreEqual("", res);

            byte[] bytes = new byte[0];
            res = CrystalConversion.FromInternal(bytes, FieldFormatting.BINARIO);
            Assert.AreEqual("", res);
            */

            //
            // FieldFormatting.CARACTERES
            //

            // null --> empty string
            res = CrystalConversion.FromInternal(null, FieldFormatting.CARACTERES);
            Assert.AreEqual("\"\"", res);

            // empty string --> empty string
            res = CrystalConversion.FromInternal("", FieldFormatting.CARACTERES);
            Assert.AreEqual("\"\"", res);

            // two single quotes --> two escaped single quotes
            res = CrystalConversion.FromInternal("''", FieldFormatting.CARACTERES);
            Assert.AreEqual("\"''\"", res);

            // a couple of spaces --> a couple of spaces
            res = CrystalConversion.FromInternal("  ", FieldFormatting.CARACTERES);
            Assert.AreEqual("\"  \"", res);

            // a string surrounded by quotes and with an accented character --> same string with escaped quotes
            res = CrystalConversion.FromInternal("'Abrenúncio'!", FieldFormatting.CARACTERES);
            Assert.AreEqual("\"'ABRENÚNCIO'!\"", res);

            // a few spaces and a couple of single quotes --> same string with escaped quotes
            res = CrystalConversion.FromInternal("   ''", FieldFormatting.CARACTERES);
            Assert.AreEqual("\"   ''\"", res);

            // a string that looks like a number --> same string
            res = CrystalConversion.FromInternal("123456", FieldFormatting.CARACTERES);
            Assert.AreEqual("\"123456\"", res);

            //
            // FieldFormatting.DATA
            //

            // null --> exception
            Assert.Throws<NullReferenceException>(() =>
                CrystalConversion.FromInternal(null, FieldFormatting.DATA)
            );

            // DateTime.MinValue --> NULL
            res = CrystalConversion.FromInternal(DateTime.MinValue, FieldFormatting.DATA);
            Assert.AreEqual("", res);

            // empty string --> NULL should throw exception
            Assert.Throws<FrameworkException>(() =>
                CrystalConversion.FromInternal("", FieldFormatting.DATA)
            );

            //
            // FieldFormatting.DATAHORA
            //

            //
            // FieldFormatting.DATASEGUNDO
            //

            //
            // FieldFormatting.DIA_MES_ANO
            //

            //
            // FieldFormatting.FLOAT
            //

            //
            // FieldFormatting.GUID
            //

            // empty GUID --> empty string
            res = CrystalConversion.FromInternal(System.Guid.Empty, FieldFormatting.GUID);
            Assert.AreEqual("\"{00000000-0000-0000-0000-000000000000}\"", res);

            // zeroed GUID --> empty string
            res = CrystalConversion.FromInternal("00000000-0000-0000-0000-000000000000", FieldFormatting.GUID);
            Assert.AreEqual("\"{00000000-0000-0000-0000-000000000000}\"", res);

            // empty string --> empty string
            res = CrystalConversion.FromInternal("", FieldFormatting.CARACTERES);
            Assert.AreEqual("\"\"", res);

            // two double quotes --> ???
            res = CrystalConversion.FromInternal("\"\"", FieldFormatting.CARACTERES);
            Assert.AreEqual("\"\"\"\"", res);

            //
            // FieldFormatting.INTEIRO
            //

            //
            // FieldFormatting.JPEG
            //

            //
            // FieldFormatting.LOGICO
            //
        }
    }
}
