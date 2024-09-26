using NUnit.Framework;

using CSGenio.business;
using CSGenio.persistence;
using CSGenio.framework;

namespace WebTest
{
    /// <summary>
    ///This is a test class for Test and is intended
    ///to contain all Test Unit Tests
    ///</summary>
    public class TestConversionDb
    {

        /// <summary>
        /// Teste à função ToString
        /// </summary>
        [Test]
        public void TestToString()
        {
            string res = null;
            string testStr;
            string expectedStr;

            // null --> empty string
            res = DBConversion.ToString(null);
            Assert.AreEqual("", res);

            // DBNull --> empty string
            res = DBConversion.ToString(DBNull.Value);
            Assert.AreEqual("", res);

            // empty GUID --> ??
            //res = DBConversion.ToString(Guid.Empty);
            //Assert.AreEqual("", res); // Actualmente retorna 00000000-0000-0000-0000-000000000000. Será suposto?

            // empty string --> empty string
            res = DBConversion.ToString("");
            Assert.AreEqual("", res);

            // string with two double quotes --> same string
            testStr = "\"\"";
            res = DBConversion.ToString(testStr);
            Assert.AreEqual(testStr, res);

            // string with two single quotes --> same string
            testStr = "''";
            res = DBConversion.ToString(testStr);
            Assert.AreEqual(testStr, res);

            // a relatively long string containing Lorem ipsum in English --> same string
            testStr = "Nor again is there anyone who loves or pursues or desires to obtain pain of itself, because it is pain, but occasionally circumstances occur in which toil and pain can procure him some great pleasure. To take a trivial example, which of us ever undertakes laborious physical exercise, except to obtain some advantage from it? But who has any right to find fault with a man who chooses to enjoy a pleasure that has no annoying consequences, or one who avoids a pain that produces no resultant pleasure?";
            res = DBConversion.ToString(testStr);
            Assert.AreEqual(testStr, res);

            // another relatively long string with accented characters --> same string
            testStr = "Este texto contém acentuação; é uma pequena experiência, ok? Vamos juntar mais alguns caracteres: @, €, $, £, º, ª, âêîôû, àÈìòù, æ, œ, <>, «», \\, [], ++, --, '', e já agora: ¡¢£¤¥¦§¨©ª«¬®¯°±²³´µ¶·¸¹º»¼½¾¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷øùúûüýþÿ";
            res = DBConversion.ToString(testStr);
            Assert.AreEqual(testStr, res);

            // string containing tabs and newlines --> same string
            testStr = "Tabs \t e \n newlines";
            res = DBConversion.ToString(testStr);
            Assert.AreEqual(testStr, res);

            // string containing only spaces (7) --> same string
            testStr = "       ";
            res = DBConversion.ToString(testStr);
            Assert.AreEqual(testStr, res);

            // a positive number (one million)
            res = DBConversion.ToString(1e9);
            Assert.AreEqual("1000000000", res);

            // zero
            res = DBConversion.ToString(0);
            Assert.AreEqual("0", res);

            // a negative number
            res = DBConversion.ToString(-5e8);
            Assert.AreEqual("-500000000", res);

            // a floating point number
            res = DBConversion.ToString(1.05);
            // FIXME DBConversion.ToString() should return culture-invariant representation
            expectedStr = "1.05";
            expectedStr = expectedStr.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            Assert.AreEqual(expectedStr, res);

            // another floating point number
            res = DBConversion.ToString(2.5e-4);
            // FIXME DBConversion.ToString() should return culture-invariant representation
            expectedStr = "0.00025";
            expectedStr = expectedStr.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            Assert.AreEqual(expectedStr, res);

            // smallest representable value in double precision - This test is somewhat ugly, but it should pass
            res = DBConversion.ToString(double.Epsilon);
            // FIXME DBConversion.ToString() should return culture-invariant representation
            expectedStr = double.Epsilon.ToString();
            expectedStr = expectedStr.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            Assert.AreEqual(expectedStr, res);

            // a number representable in decimal, but not in 64bit double precision
            res = DBConversion.ToString(1.0000000000000001m);
            // FIXME DBConversion.ToString() should return culture-invariant representation
            expectedStr = "1.0000000000000001";
            expectedStr = expectedStr.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            Assert.AreEqual(expectedStr, res);

            // some random object
            //try
            //{
            //    res = DBConversion.ToString(typeof(DBConversion));
            //    Assert.Fail("DBConversion.ToString(<some random object>) should throw an exception");
            //}
            //catch (AssertFailedException)
            //{
            //    throw;
            //}
            //catch { }
        }

        /// <summary>
        /// Teste à função FromString
        /// </summary>
        [Test]
        public void TestFromString()
        {
            string res = null;

            // null --> empty string
            res = DBConversion.FromString(null);
            // Até mais ver, Qvalues "vazios" são representados por strings de comprimento zero, e não por NULL
            Assert.AreEqual("''", res);

            // empty string --> empty string
            res = DBConversion.FromString("");
            // Até mais ver, Qvalues "vazios" são representados por strings de comprimento zero, e não por NULL
            Assert.AreEqual("''", res);

            // single quote --> escaped single quote
            res = DBConversion.FromString("'");
            Assert.AreEqual("''''", res);

            // two single quotes --> two escaped single quote
            res = DBConversion.FromString("''");
            Assert.AreEqual("''''''", res);

            // single space --> single space
            res = DBConversion.FromString(" ");
            Assert.AreEqual("' '", res);

            // string with a number --> string with a number
            res = DBConversion.FromString("134");
            Assert.AreEqual("'134'", res);

            // string beginning with single quote --> same string with escaped quote
            res = DBConversion.FromString("'--; SQL Injection!");
            Assert.AreEqual("'''--; SQL Injection!'", res);

            // bunch of Unicode characters --> same bunch of Unicode characters
            res = DBConversion.FromString("¡¢£¤¥¦§¨©ª«¬®¯°±²³´µ¶·¸¹º»¼½¾¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷øùúûüýþÿ");
            Assert.AreEqual("'¡¢£¤¥¦§¨©ª«¬®¯°±²³´µ¶·¸¹º»¼½¾¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷øùúûüýþÿ'", res);
        }

        /// <summary>
        /// Teste às funções FromNumeric e ToNumeric.
        /// </summary>
        [Test]
        public void TestFromNumericAndToNumeric()
        {
            decimal res;

            // TODO: actualmente tudo o que envolve digits decimais está a falhar
            // Há que ver até que ponto faz sentido o auxFromNumericAndBack,
            // já que o ToNumeric() "supostamente" não recebe strings.

            auxFromNumericAndBack(0);
            auxFromNumericAndBack(-1);
            //auxFromNumericAndBack(double.MinValue);
            //auxFromNumericAndBack(double.MaxValue);
            //auxFromNumericAndBack(0.7);  
            //auxFromNumericAndBack(-12345678.97);
            //auxFromNumericAndBack(Math.PI);
            //auxFromNumericAndBack(Math.E);

            // empty string --> zero
            res = DBConversion.ToNumeric("");
            Assert.AreEqual(0, res);

            // "  -8 " --> -8
            res = DBConversion.ToNumeric("  -8 ");
            Assert.AreEqual(-8, res);

            // "  +110" --> 110
            res = DBConversion.ToNumeric("  +110");
            Assert.AreEqual(110, res);

            //res = DBConversion.ToNumeric(double.MaxValue.ToString());
            //Assert.AreEqual(double.MaxValue, res);

            // "4294967296. " --> 4294967296.0  [that's 2^32, since you're wondering]
            res = DBConversion.ToNumeric("4294967296. ");
            Assert.AreEqual(4294967296.0m, res);

            // "Not a Number" --> exception
            //try
            //{
            //    res = DBConversion.ToNumeric("Not a Number");
            //    Assert.Fail("should have thrown an exception, but instead returned " + res);
            //}
            //catch (AssertFailedException)
            //{
            //    throw;
            //}
            //catch { }

            //// "-+0" --> exception
            //try
            //{
            //    res = DBConversion.ToNumeric("-+0");
            //    Assert.Fail("should have thrown an exception, but instead returned " + res);
            //}
            //catch (AssertFailedException)
            //{
            //    throw;
            //}
            //catch { }
        }

        private void auxFromNumericAndBack(decimal original)
        {
            string dbVal = DBConversion.FromNumeric(original);
            decimal result = DBConversion.ToNumeric(dbVal);
            Assert.AreEqual(original, result, String.Format("Expected {0} but got {1}. Database value was {2}", original, result, dbVal));
        }

        /// <summary>
        /// Teste à função ToInteger
        /// </summary>
        [Test]
        public void TestToInteger()
        {
            int res;

            // null --> 0
            res = DBConversion.ToInteger(null);
            Assert.AreEqual(0, res);

            //
            // From booleans
            //

            // false --> 0
            res = DBConversion.ToInteger(false);
            Assert.AreEqual(0, res);

            // true --> 1
            res = DBConversion.ToInteger(true);
            Assert.AreEqual(1, res);

            //
            // From numeric types
            //

            // int.MinValue --> int.MinValue
            res = DBConversion.ToInteger(int.MinValue);
            Assert.AreEqual(int.MinValue, res);

            // 0 --> 0
            res = DBConversion.ToInteger(0);
            Assert.AreEqual(0, res);

            // int.MaxValue --> int.MaxValue
            res = DBConversion.ToInteger(int.MinValue);
            Assert.AreEqual(int.MinValue, res);

            // double.MinValue --> int.MinValue
            //res = DBConversion.ToInteger(double.MinValue); // FIXME está a falhar
            //Assert.AreEqual(int.MinValue, res);

            // double.MaxValue --> int.MaxValue
            //res = DBConversion.ToInteger(double.MaxValue); // FIXME está a falhar
            //Assert.AreEqual(int.MaxValue, res);

            // decimal.MinValue --> int.MinValue
            //res = DBConversion.ToInteger(decimal.MinValue); // FIXME está a falhar
            //Assert.AreEqual(int.MinValue, res);

            // decimal.MaxValue --> int.MaxValue
            //res = DBConversion.ToInteger(decimal.MaxValue); // FIXME está a falhar
            //Assert.AreEqual(int.MaxValue, res);

            //
            // From strings
            //

            // "-1" --> -1
            res = DBConversion.ToInteger("-1");
            Assert.AreEqual(-1, res);

            // "0" --> 0
            res = DBConversion.ToInteger("0");
            Assert.AreEqual(0, res);

            // int.MinValue --> int.MinValue
            res = DBConversion.ToInteger(int.MinValue);
            Assert.AreEqual(int.MinValue, res);

            // 0 --> 0
            res = DBConversion.ToInteger(0);
            Assert.AreEqual(0, res);

            // int.MaxValue --> int.MaxValue
            res = DBConversion.ToInteger(int.MaxValue);
            Assert.AreEqual(int.MaxValue, res);

            // "   42   " --> 42
            res = DBConversion.ToInteger("   42   ");
            Assert.AreEqual(42, res);

            // " 8 7  " --> exception
            //try
            //{
            //    res = DBConversion.ToInteger(" 8 7  ");
            //    Assert.Fail("DBConversion.ToInteger(\" 8 7  \") should have thrown an exception, but instead returned " + res);
            //}
            //catch (AssertFailedException)
            //{
            //    throw;
            //}
            //catch { }

            //// "NaN" --> exception
            //try
            //{
            //    res = DBConversion.ToInteger("NaN");
            //    Assert.Fail("DBConversion.ToInteger(\"NaN\") should have thrown an exception, but instead returned " + res);
            //}
            //catch (AssertFailedException)
            //{
            //    throw;
            //}
            //catch { }

            //// empty string --> exception
            //try
            //{
            //    res = DBConversion.ToInteger("");
            //    Assert.Fail("DBConversion.ToInteger(\"\") should have thrown an exception, but instead returned " + res);
            //}
            //catch (AssertFailedException)
            //{
            //    throw;
            //}
            //catch { }
        }

        /// <summary>
        /// Teste à função FromInteger
        /// </summary>
        [Test]
        public void TestFromInteger()
        {
            // Como DBConversion.FromInteger é essencialmente um Convert.ToString(int),
            // este test é algo ingrato

            string res = null;

            res = DBConversion.FromInteger(0);
            Assert.AreEqual("0", res);

            res = DBConversion.FromInteger(-2);
            Assert.AreEqual("-2", res);

            res = DBConversion.FromInteger(9000);
            Assert.AreEqual("9000", res);
        }

        /// <summary>
        /// Teste à função ToDateTime
        /// </summary>
        [Test]
        public void TestToDateTime()
        {
            DateTime res;
            DateTime expected;

            // null --> DateTime.MinValue
            res = DBConversion.ToDateTime(null);
            Assert.AreEqual(DateTime.MinValue, res);

            // DateTime.MinValue --> DateTime.MinValue
            res = DBConversion.ToDateTime(DateTime.MinValue);
            Assert.AreEqual(DateTime.MinValue, res);

            // DBNull.Value --> DateTime.MinValue
            res = DBConversion.ToDateTime(DBNull.Value);
            Assert.AreEqual(DateTime.MinValue, res);

            // DateTime.Now ok
            expected = DateTime.Now;
            res = DBConversion.ToDateTime(expected);
            Assert.AreEqual(expected, res);

            // DateTime.Today ok
            expected = DateTime.Today;
            res = DBConversion.ToDateTime(expected);
            Assert.AreEqual(expected, res);

            // 1900-01-01 ok
            expected = new DateTime(1900, 1, 1);
            res = DBConversion.ToDateTime(expected);
            Assert.AreEqual(expected, res);

            // 1999-12-31 ok
            expected = new DateTime(1999, 12, 31);
            res = DBConversion.ToDateTime(expected);
            Assert.AreEqual(expected, res);

            // 2012-12-22 ok [the world has ended yet]
            expected = new DateTime(2012, 12, 22);
            res = DBConversion.ToDateTime(expected);
            Assert.AreEqual(expected, res);

            // DateTime.MaxValue ok
            res = DBConversion.ToDateTime(DateTime.MaxValue);
            Assert.AreEqual(DateTime.MaxValue, res);

            // a string --> exception
            Assert.Throws<FrameworkException>(() => {
                res = DBConversion.ToDateTime(DateTime.Now.ToString());
            });

            // empty string --> exception
            Assert.Throws<FrameworkException>(() => {
                res = DBConversion.ToDateTime("");
            });

            // an integer --> exception
            Assert.Throws<FrameworkException>(() => {
                res = DBConversion.ToDateTime(20110411);
            });
        }

        /// <summary>
        ///Teste à função FromDateTime
        /// </summary>
        [Test]
        public void TestFromDateTime()
        {
            string res = null;

            res = DBConversion.FromDateTime(DateTime.MinValue, DatabaseType.ACCESS);
            Assert.AreEqual("NULL", res);

            res = DBConversion.FromDateTime(new DateTime(1900, 2, 1), DatabaseType.SQLSERVER2000);
            Assert.AreEqual("convert(datetime, '1/2/1900 00:00:00', 103)", res);

            res = DBConversion.FromDateTime(new DateTime(1900, 2, 1), DatabaseType.ORACLE);
            Assert.AreEqual("TO_DATE('1900/2/1 0:0:0', 'YYYY/MM/DD hh24:mi:ss')", res);
        }

        /// <summary>
        /// Teste à função ToLogic
        /// </summary>
        [Test]
        public void TestToLogic()
        {
            int res = -1;

            // DBNull.Value --> 0
            res = DBConversion.ToLogic(DBNull.Value);
            Assert.AreEqual(0, res);

            // null --> 0
            res = DBConversion.ToLogic(null);
            Assert.AreEqual(0, res);

            //
            // From booleans
            //

            // false --> 0
            res = DBConversion.ToLogic(false);
            Assert.AreEqual(0, res);

            // true --> 1
            res = DBConversion.ToLogic(true);
            Assert.AreEqual(1, res);

            //
            // From integers
            //

            // 0 --> 0
            res = DBConversion.ToLogic(0);
            Assert.AreEqual(0, res);

            // 1 --> 1
            res = DBConversion.ToLogic(1);
            Assert.AreEqual(1, res);

            // 42 --> 1
            //res = DBConversion.ToLogic(42);  // FIXME está a falhar. Será que devia passar?
            //Assert.AreEqual(1, res);
        }

        /// <summary>
        /// Teste à função FromLogic
        /// </summary>
        [Test]
        public void TestFromLogic()
        {
            string res = null;

            res = DBConversion.FromLogic(int.MinValue);
            //Assert.AreEqual(?, res);

            res = DBConversion.FromLogic(-1);
            Assert.AreEqual("-1", res); // Ou erro?

            res = DBConversion.FromLogic(0);
            Assert.AreEqual("0", res);

            res = DBConversion.FromLogic(1);
            Assert.AreEqual("1", res);

            res = DBConversion.FromLogic(int.MaxValue);
            //Assert.AreEqual(?, res);
        }

        /// <summary>
        /// Teste à função ToKey
        /// </summary>
        [Test]
        public void TestToKey()
        {
            string res = null;

            // null --> empty string
            res = DBConversion.ToKey(null);
            Assert.AreEqual("", res);

            // empty string --> empty string
            res = DBConversion.ToKey(String.Empty);
            Assert.AreEqual("", res);

            // DBNull --> empty string
            res = DBConversion.ToKey(DBNull.Value);
            Assert.AreEqual("", res);

            // empty GUID --> empty string
            res = DBConversion.ToKey(Guid.Empty);
            Assert.AreEqual("", res);

            // zeroed GUID as a SQL string --> empty string
            res = DBConversion.ToKey("'00000000-0000-0000-0000-000000000000'"); // FIXME está a falhar. Será suposto?
            Assert.AreEqual("", res);

            // non-empty empty GUID as a SQL string --> same GUID (with quotes removed)
            res = DBConversion.ToKey("'a6bb0086-8c37-4b88-8235-161f76134a2a'");
            Assert.AreEqual("a6bb0086-8c37-4b88-8235-161f76134a2a", res);

        }

        /// <summary>
        /// Teste à função FromKey
        /// </summary>
        [Test]
        public void TestFromKey()
        {
            string res = null;

            // empty string --> NULL
            res = DBConversion.FromKey(string.Empty);
            Assert.AreEqual("NULL", res);

            // null --> NULL
            res = DBConversion.FromKey(null);
            Assert.AreEqual("NULL", res);

            // empty GUID (as string) --> NULL
            res = DBConversion.FromKey(Guid.Empty.ToString());
            Assert.AreEqual("NULL", res);

            // zeroed GUID (no quotes) --> NULL
            res = DBConversion.FromKey("00000000-0000-0000-0000-000000000000");
            Assert.AreEqual("NULL", res);

            // non-empty GUID (no quotes) --> same GUID (quoted)
            res = DBConversion.FromKey("a6bb0086-8c37-4b88-8235-161f76134a2a");
            Assert.AreEqual("'a6bb0086-8c37-4b88-8235-161f76134a2a'", res);

        }

        /// <summary>
        /// Teste à função ToBinary
        /// </summary>
        [Test]
        public void TestToBinary()
        {

        }

        /// <summary>
        /// Teste à função FromBinary
        /// </summary>
        [Test]
        public void TestFromBinary()
        {
        }

        /// <summary>
        /// Teste às funções ToInternal e FromInternal
        /// </summary>
        [Test]
        public void TestToInterno()
        {
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
            object res = null;

            //
            // FieldFormatting.ANO_MES_DIA *** Não é usado
            //
            //res = DBConversion.ToInternal(null, FieldFormatting.ANO_MES_DIA);
            //Assert.IsNull(res);

            //
            // FieldFormatting.BINARIO
            //

            // null --> zero-length byte array
            res = DBConversion.ToInternal(null, FieldFormatting.BINARIO);
            Assert.IsInstanceOf(typeof(byte[]), res);
            Assert.IsTrue((res as byte[]).Length == 0);

            // DBNull --> zero-length byte array
            res = DBConversion.ToInternal(DBNull.Value, FieldFormatting.BINARIO);
            Assert.IsInstanceOf(typeof(byte[]), res);
            Assert.IsTrue((res as byte[]).Length == 0);

            //
            // FieldFormatting.CARACTERES
            //

            // null --> empty string
            res = DBConversion.ToInternal(null, FieldFormatting.CARACTERES);
            Assert.IsInstanceOf(typeof(string), res);
            Assert.AreEqual("", res);

            //
            // FieldFormatting.DATA
            //

            // null --> DateTime.MinValue
            res = DBConversion.ToInternal(null, FieldFormatting.DATA);
            Assert.IsInstanceOf(typeof(DateTime), res);
            Assert.AreEqual(DateTime.MinValue, res);

            // DateTime.MinValue --> DateTime.MinValue
            res = DBConversion.ToInternal(DateTime.MinValue, FieldFormatting.DATA);
            Assert.IsInstanceOf(typeof(DateTime), res);
            Assert.AreEqual(DateTime.MinValue, res);

            // DBNull --> DateTime.MinValue
            res = DBConversion.ToInternal(DBNull.Value, FieldFormatting.DATA);
            Assert.IsInstanceOf(typeof(DateTime), res);
            Assert.AreEqual(DateTime.MinValue, res);

            //
            // FieldFormatting.DATAHORA;
            //

            //
            // FieldFormatting.DATASEGUNDO;
            //

            //
            // FieldFormatting.DIA_MES_ANO;
            //

            //
            // FieldFormatting.FLOAT;
            //

            // null --> 0.0
            res = DBConversion.ToInternal(null, FieldFormatting.FLOAT);
            Assert.AreEqual(0m, res);

            // DBNull --> 0.0
            res = DBConversion.ToInternal(DBNull.Value, FieldFormatting.FLOAT);
            Assert.AreEqual(0m, res);

            // With conversion to String with current culture
            res = DBConversion.ToInternal(0.9464572.ToString(), FieldFormatting.FLOAT);
            Assert.AreEqual(0.9464572m, res);

            // With conversion to string in Invariant culture
            res = DBConversion.ToInternal(0.9464572.ToString(System.Globalization.CultureInfo.InvariantCulture), FieldFormatting.FLOAT); 
            Assert.AreEqual(0.9464572m, res);

            res = DBConversion.ToInternal(0.987654321, FieldFormatting.FLOAT);
            Assert.AreEqual(0.987654321m, res);

            //
            // FieldFormatting.GUID;
            //

            //
            // FieldFormatting.INTEIRO;
            //

            //
            // FieldFormatting.JPEG;
            //

            //
            // FieldFormatting.LOGICO;
            //
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
            res = DBConversion.FromInternal(null, FieldFormatting.BINARIO);
            Assert.AreEqual("NULL", res);

            res = DBConversion.FromInternal("", FieldFormatting.BINARIO);
            Assert.AreEqual("", res);

            byte[] bytes = new byte[0];
            res = DBConversion.FromInternal(bytes, FieldFormatting.BINARIO);
            Assert.AreEqual("", res);
            */

            //
            // FieldFormatting.CARACTERES
            //

            // null --> empty string
            res = DBConversion.FromInternal(null, FieldFormatting.CARACTERES);
            Assert.AreEqual("''", res);

            // empty string --> empty string
            res = DBConversion.FromInternal("", FieldFormatting.CARACTERES);
            Assert.AreEqual("''", res);

            // two single quotes --> two escaped single quotes
            res = DBConversion.FromInternal("''", FieldFormatting.CARACTERES);
            Assert.AreEqual("''''''", res);

            // a couple of spaces --> a couple of spaces
            res = DBConversion.FromInternal("  ", FieldFormatting.CARACTERES);
            Assert.AreEqual("'  '", res);

            // a string surrounded by quotes and with an accented character --> same string with escaped quotes
            res = DBConversion.FromInternal("'Abrenúncio'!", FieldFormatting.CARACTERES);
            Assert.AreEqual("'''Abrenúncio''!'", res);

            // a few spaces and a couple of single quotes --> same string with escaped quotes
            res = DBConversion.FromInternal("   ''", FieldFormatting.CARACTERES);
            Assert.AreEqual("'   '''''", res);

            // a string that looks like a number --> same string
            res = DBConversion.FromInternal("123456", FieldFormatting.CARACTERES);
            Assert.AreEqual("'123456'", res);

            //
            // FieldFormatting.DATA
            //

            // null --> exception
            Assert.Throws<FrameworkException>(() => DBConversion.FromInternal(null, FieldFormatting.DATA));

            // DateTime.MinValue --> NULL
            res = DBConversion.FromInternal(DateTime.MinValue, FieldFormatting.DATA);
            Assert.AreEqual("NULL", res);

            // empty string --> NULL should throw exception
            Assert.Throws<FrameworkException>(() => DBConversion.FromInternal("", FieldFormatting.DATA));

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
            res = DBConversion.FromInternal(System.Guid.Empty, FieldFormatting.BINARIO);
            Assert.AreEqual("''", res);

            // zeroed GUID --> empty string
            res = DBConversion.FromInternal("00000000-0000-0000-0000-000000000000", FieldFormatting.BINARIO);
            Assert.AreEqual("''", res);

            // empty string --> empty string
            res = DBConversion.FromInternal("''", FieldFormatting.BINARIO);
            Assert.AreEqual("''", res);

            // two double quotes --> ???
            res = DBConversion.FromInternal("\"\"", FieldFormatting.BINARIO);
            Assert.AreEqual("''", res);

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
