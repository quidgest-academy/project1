using NUnit.Framework;

using CSGenio.business;
using CSGenio.framework;

namespace WebTest
{
    /// <summary>
    /// Summary description for TestFuncoesGlobais
    /// </summary>
    public class TestFuncoesGlobais
    {
        public TestFuncoesGlobais()
        {
            //
            // TODO: Add constructor logic here
            //
        }
/*

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }
*/
        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [Test]
        public void TestSomaDias()
        {
            DateTime res = GlobalFunctions.SumDays(new DateTime(2010,10,31), 1);
            Assert.AreEqual(new DateTime(2010,11,01), res);
            res = GlobalFunctions.SumDays(new DateTime(2010,12,01), 31);
            Assert.AreEqual(new DateTime(2011,01,01), res);
            res = GlobalFunctions.SumDays(new DateTime(2010,12,01), 0);
            Assert.AreEqual(new DateTime(2010,12,01), res);
            res = GlobalFunctions.SumDays(new DateTime(2010,12,01), -1);
            Assert.AreEqual(new DateTime(2010,11,30), res);
            res = GlobalFunctions.SumDays(DateTime.MinValue, 1);
            Assert.AreEqual(DateTime.MinValue, res);
        }

        [Test]
        public void TestAtoi()
        {
            int res = GlobalFunctions.atoi("1234");
            Assert.AreEqual(1234, res);
            res = GlobalFunctions.atoi("");
            Assert.AreEqual(0, res);
            res = GlobalFunctions.atoi(null);
            Assert.AreEqual(0, res);
            res = GlobalFunctions.atoi("-4567");
            Assert.AreEqual(-4567, res);
            Assert.Throws<FormatException>(() => {
                res = GlobalFunctions.atoi("98,76");
            });
            Assert.Throws<FormatException>(() => {
                res = GlobalFunctions.atoi("xpto");
            });
		}

        [Test]
        public void TestIntToString()
        {
            string res = GlobalFunctions.IntToString(0);
            Assert.AreEqual("0", res);
            res = GlobalFunctions.IntToString(-1);
            Assert.AreEqual("-1", res);
            //res = GlobalFunctions.IntToString(0.5);
            //Assert.AreEqual("0", res); //Não devia truncar?
        }

        [Test]
        public void TestNumericToString()
        {
            //This is all kinds of wrong. Standard functions should not depend on current culture
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            string res = GlobalFunctions.NumericToString(23,3);
            Assert.AreEqual("23", res); //Não devia ser 23.000?
            res = GlobalFunctions.NumericToString(23.123m, 3);
            Assert.AreEqual("23.123", res); //Devia sair com virgula?
            res = GlobalFunctions.NumericToString(100.123m, 0);
            Assert.AreEqual("100", res);
            res = GlobalFunctions.NumericToString(-100.123m, 1);
            Assert.AreEqual("-100.1", res);
        }

        [Test]
        public void TestEmptyD()
        {
            int res = GlobalFunctions.emptyD(DateTime.MinValue);
            Assert.AreEqual(1, res);
            res = GlobalFunctions.emptyD(new DateTime(2010,10,31));
            Assert.AreEqual(0, res);
            res = GlobalFunctions.emptyD(DateTime.MaxValue);
            Assert.AreEqual(0, res);
            res = GlobalFunctions.emptyD(null);
            Assert.AreEqual(1, res);
        }

        [Test]
        public void TestEmptyG()
        {
            int res = GlobalFunctions.emptyG(Guid.Empty.ToString());
            Assert.AreEqual(1, res);
            res = GlobalFunctions.emptyG(String.Empty);
            Assert.AreEqual(1, res);
            res = GlobalFunctions.emptyG("1234");
            Assert.AreEqual(0, res);
            res = GlobalFunctions.emptyG(Guid.NewGuid().ToString());
            Assert.AreEqual(0, res);
            res = GlobalFunctions.emptyG(null);
            Assert.AreEqual(1, res);
        }

        [Test]
        public void TestEmptyC()
        {
            int res = GlobalFunctions.emptyC(String.Empty);
            Assert.AreEqual(1, res);
            res = GlobalFunctions.emptyC("1234");
            Assert.AreEqual(0, res);
            res = GlobalFunctions.emptyC(null);
            Assert.AreEqual(1, res);
        }

        [Test]
        public void TestEmptyN()
        {
            int res = GlobalFunctions.emptyN(0.0);
            Assert.AreEqual(1, res);
            res = GlobalFunctions.emptyN(double.MinValue);
            Assert.AreEqual(0, res);
            res = GlobalFunctions.emptyN(1234.00);
            Assert.AreEqual(0, res);
            res = GlobalFunctions.emptyN(null);
            Assert.AreEqual(1, res);
        }
        
        [Test]
        public void TestEmptyT()
        {
            int res = GlobalFunctions.emptyT(null);
            Assert.AreEqual(1, res);
            res = GlobalFunctions.emptyT("__:__");
            Assert.AreEqual(1, res);
            res = GlobalFunctions.emptyT(String.Empty);
            Assert.AreEqual(1, res);
            res = GlobalFunctions.emptyT("12:34");
            Assert.AreEqual(0, res);
        }
        
        [Test]
        public void TestEmptyL()
        {
            int res = GlobalFunctions.emptyL(0);
            Assert.AreEqual(1, res);
            res = GlobalFunctions.emptyL(1);
            Assert.AreEqual(0, res);
            res = GlobalFunctions.emptyL(null);
            Assert.AreEqual(1, res);
        }

        [Test]
        public void TestLTRIM()
        {
            string res = GlobalFunctions.LTRIM("tre jolie");
            Assert.AreEqual("tre jolie", res);
            res = GlobalFunctions.LTRIM(" \r\n tre jolie");
            Assert.AreEqual("tre jolie", res);
            res = GlobalFunctions.LTRIM("tre jolie \r\n ");
            Assert.AreEqual("tre jolie \r\n ", res);
            res = GlobalFunctions.LTRIM(" \r\n tre jolie \r\n ");
            Assert.AreEqual("tre jolie \r\n ", res);
            res = GlobalFunctions.LTRIM("");
            Assert.AreEqual("", res);
        }

        [Test]
        public void TestRTRIM()
        {
            string res = GlobalFunctions.RTRIM("tre jolie");
            Assert.AreEqual("tre jolie", res);
            res = GlobalFunctions.RTRIM(" \r\n tre jolie");
            Assert.AreEqual(" \r\n tre jolie", res);
            res = GlobalFunctions.RTRIM("tre jolie \r\n ");
            Assert.AreEqual("tre jolie", res);
            res = GlobalFunctions.RTRIM(" \r\n tre jolie \r\n ");
            Assert.AreEqual(" \r\n tre jolie", res);
            res = GlobalFunctions.RTRIM("");
            Assert.AreEqual("", res);
        }

        [Test]
        public void TestYear()
        {
            int res = GlobalFunctions.Year(new DateTime(2012,12,14));
            Assert.AreEqual(2012, res);
            res = GlobalFunctions.Year(DateTime.MinValue);
            Assert.AreEqual(0, res);
        }

        [Test]
        public void TestMonth()
        {
            int res = GlobalFunctions.Month(new DateTime(2012,12,14));
            Assert.AreEqual(12, res);
            res = GlobalFunctions.Month(DateTime.MinValue);
            Assert.AreEqual(0, res); //devia mesmo ser 0? não devia ser 1?
        }

        [Test]
        public void TestDay()
        {
            int res = GlobalFunctions.Day(new DateTime(2012,12,14));
            Assert.AreEqual(14, res);
            res = GlobalFunctions.Day(DateTime.MinValue);
            Assert.AreEqual(0, res); //devia mesmo ser 0? não devia ser 1?
        }

        [Test]
        public void TestHorasToDouble()
        {
            decimal res = GlobalFunctions.HoursToDouble("10:30");
            Assert.AreEqual(10 + 30/60m, res);
            res = GlobalFunctions.HoursToDouble("10:01");
            Assert.AreEqual(10 + 01 / 60m, res);
            res = GlobalFunctions.HoursToDouble("_1:_1");
            Assert.AreEqual(01 + 01 / 60m, res);
            res = GlobalFunctions.HoursToDouble("__:__");
            Assert.AreEqual(0, res);
            res = GlobalFunctions.HoursToDouble("");
            Assert.AreEqual(0, res);

            res = GlobalFunctions.HoursToDouble("25:30");
            Assert.AreEqual(0, res);
            res = GlobalFunctions.HoursToDouble("12:70");
            Assert.AreEqual(0, res);

            //TODO: null
        }

        [Test]
        public void TestDoubleToHoras()
        {
            string res = GlobalFunctions.DoubleToHours(10 + 30 / 60.0m);
            Assert.AreEqual("10:30", res);
            res = GlobalFunctions.DoubleToHours(10 + 01.0m / 60.0m);
            Assert.AreEqual("10:01", res);
            res = GlobalFunctions.DoubleToHours(0.0m);
            Assert.AreEqual("00:00", res);
            res = GlobalFunctions.DoubleToHours(2.999m);
            Assert.AreEqual("03:00", res);
            res = GlobalFunctions.DoubleToHours(2.991m);
            Assert.AreEqual("02:59", res);
            //TODO: negative
            //TODO: out of bounds
            //TODO: a aplicação devia crashar em debug mas apenas fazer log de um erro em produção de forma a não perder dados
        }

        [Test]
        public void TestHorasAdd()
        {
            string res = GlobalFunctions.HoursAdd("00:00", 1);
            Assert.AreEqual("00:01", res);
            res = GlobalFunctions.HoursAdd("02:00", -1);
            Assert.AreEqual("01:59", res);
            res = GlobalFunctions.HoursAdd("02:03", 57);
            Assert.AreEqual("03:00", res);
            res = GlobalFunctions.HoursAdd("00:00", 24 * 60);
            Assert.AreEqual("23:59", res);
            res = GlobalFunctions.HoursAdd("23:59", -24 * 60);
            Assert.AreEqual("00:00", res);
            res = GlobalFunctions.HoursAdd("20:5_", 5);
            Assert.AreEqual("20:55", res);
            res = GlobalFunctions.HoursAdd("20:5", 5);
            Assert.AreEqual("__:__", res);
            res = GlobalFunctions.HoursAdd(null, 5);
            Assert.AreEqual("__:__", res);
        }

        [Test]
        public void TestHorasDoubleRoundtrip()
        {
            //diferenca entre 2 horas
            decimal res = GlobalFunctions.HoursToDouble("00:30") - GlobalFunctions.HoursToDouble("00:29");
            //Assert.AreEqual(00.0 + 01.0 / 60.0, res); //Não pode dar exactamente igual por causa das aproximações
            //o que interessa é que seja recuperável de volta a uma string de horas
            Assert.AreEqual("00:01", GlobalFunctions.DoubleToHours(res));

            res = GlobalFunctions.HoursToDouble("23:59") - GlobalFunctions.HoursToDouble("23:58");
            Assert.AreEqual("00:01", GlobalFunctions.DoubleToHours(res));

            res = GlobalFunctions.HoursToDouble("23:59") - GlobalFunctions.HoursToDouble("12:01");
            Assert.AreEqual("11:58", GlobalFunctions.DoubleToHours(res));

            res = GlobalFunctions.HoursToDouble("23:59") - GlobalFunctions.HoursToDouble("00:01");
            Assert.AreEqual("23:58", GlobalFunctions.DoubleToHours(res));
        }

        [Test]
        public void TestCreateDateTime()
        {
            DateTime minDate = DateTime.MinValue;
            
            DateTime res = GlobalFunctions.CreateDateTime(2010, 10, 30, 20, 30, 00);
            Assert.AreEqual(new DateTime(2010, 10, 30, 20, 30, 00), res);

            res = GlobalFunctions.CreateDateTime(minDate.Year, minDate.Month, minDate.Day, 00, 00, 00);
            Assert.AreEqual(minDate, res);

            // when an error occurs, CreateDateTime returns DateTime.MinValue
            res = GlobalFunctions.CreateDateTime(2010, 10, 30, 30, 30, 00);
            Assert.AreEqual(minDate, res);

            res = GlobalFunctions.CreateDateTime(2010, 10, 30, 20, 80, 00);
            Assert.AreEqual(minDate, res);
        }
        
        [Test]
        public void TestDateSetTime()
        {
            DateTime minDate = DateTime.MinValue;
            DateTime baseDate = GlobalFunctions.CreateDateTime(2010, 10, 30, 00, 00, 00);
            DateTime res;

            // if passed a DateTime.MinValue, it will not evaluate the time
            res = GlobalFunctions.DateSetTime(minDate, "20:30");
            Assert.AreEqual(minDate, res);

            res = GlobalFunctions.DateSetTime(baseDate, "20:30");
            Assert.AreEqual(new DateTime(2010,10,30,20,30,00), res);

            // invalid time will be ignored and original date returned
            res = GlobalFunctions.DateSetTime(baseDate, "30:30");
            Assert.AreEqual(baseDate, res);
            res = GlobalFunctions.DateSetTime(baseDate, "20:80");
            Assert.AreEqual(baseDate, res);
        }

        /*
        // Deprecated
        [Test]
        public void TestCriaDataHora()
        {
            DateTime res = GlobalFunctions.CriaDataHora(new DateTime(2010,10,30), "20:30");
            Assert.AreEqual(new DateTime(2010,10,30, 20,30,00), res);

            res = GlobalFunctions.CriaDataHora(DateTime.MinValue, "20:30");
            Assert.AreEqual(DateTime.MinValue, res);

            res = GlobalFunctions.CriaDataHora(new DateTime(2010,10,30), "30:30");
            Assert.AreEqual(new DateTime(2010,10,30, 00,00,00), res);
            res = GlobalFunctions.CriaDataHora(new DateTime(2010,10,30), "20:80");
            Assert.AreEqual(new DateTime(2010,10,30, 00,00,00), res);
        }
        */

        /*
        // Deprecated
        [Test]
        public void TestIsValid()
        {
            int res;
            res = GlobalFunctions.IsValid(DateTime.MinValue);
            Assert.AreEqual(0, res);
            res = GlobalFunctions.IsValid(DateTime.MaxValue);
            Assert.AreEqual(1, res);
            res = GlobalFunctions.IsValid(new DateTime(2012,12,14));
            Assert.AreEqual(1, res);
        }
        */

        [Test]
        public void TestKeyToString()
        {
            string res = GlobalFunctions.KeyToString("    1");
            Assert.AreEqual("    1", res);
            //test lower case
            res = GlobalFunctions.KeyToString("{234dceae-7c12-40e9-bbf5-b59f5f6dd890}");
            Assert.AreEqual("234DCEAE7C1240E9BBF5B59F5F6DD890", res);            
            //test upper case
            res = GlobalFunctions.KeyToString("{234DCEAE-7C12-40E9-BBF5-B59F5F6DD890}");
            Assert.AreEqual("234DCEAE7C1240E9BBF5B59F5F6DD890", res);

            //TODO: null
        }

        [Test]
        public void TestMinN()
        {
            decimal res = GlobalFunctions.minN(0.0m, -1.0m);
            Assert.AreEqual(-1, res);
            res = GlobalFunctions.minN(-1.0m, 0.0m);
            Assert.AreEqual(-1, res);
            res = GlobalFunctions.minN(2.0m, 2.0m);
            Assert.AreEqual(2, res);
        }

        [Test]
        public void TestMaxN()
        {
            decimal res = GlobalFunctions.maxN(0.0m, -1.0m);
            Assert.AreEqual(0, res);
            res = GlobalFunctions.maxN(-1.0m, 0.0m);
            Assert.AreEqual(0, res);
            res = GlobalFunctions.maxN(2.0m, 2.0m);
            Assert.AreEqual(2, res);
        }

        [Test]
        public void TestMinD()
        {
            DateTime res = GlobalFunctions.minD(new DateTime(2010,10,30), new DateTime(2010,10,31));
            Assert.AreEqual(new DateTime(2010,10,30), res);
            res = GlobalFunctions.minD(new DateTime(2010,10,31), new DateTime(2010,10,30));
            Assert.AreEqual(new DateTime(2010,10,30), res);
            res = GlobalFunctions.minD(new DateTime(2010,10,31), new DateTime(2010,10,31));
            Assert.AreEqual(new DateTime(2010,10,31), res);
            res = GlobalFunctions.minD(DateTime.MinValue, new DateTime(2010,10,31));
            Assert.AreEqual(DateTime.MinValue, res);
        }

        [Test]
        public void TestMaxD()
        {
            DateTime res = GlobalFunctions.maxD(new DateTime(2010,10,30), new DateTime(2010,10,31));
            Assert.AreEqual(new DateTime(2010,10,31), res);
            res = GlobalFunctions.maxD(new DateTime(2010,10,31), new DateTime(2010,10,30));
            Assert.AreEqual(new DateTime(2010,10,31), res);
            res = GlobalFunctions.maxD(new DateTime(2010,10,31), new DateTime(2010,10,31));
            Assert.AreEqual(new DateTime(2010,10,31), res);
            res = GlobalFunctions.maxD(DateTime.MinValue, new DateTime(2010,10,31));
            Assert.AreEqual(new DateTime(2010,10,31), res);
        }

        [Test]
        public void TestLEFT()
        {
            string res = GlobalFunctions.LEFT("ola adeus", 3);
            Assert.AreEqual("ola", res);
            res = GlobalFunctions.LEFT("ola adeus", 30);
            Assert.AreEqual("ola adeus", res);
            res = GlobalFunctions.LEFT("ola adeus", -1);
            Assert.AreEqual("", res);
            res = GlobalFunctions.LEFT(null, 10);
            Assert.AreEqual("", res);
        }

        [Test]
        public void TestRIGHT()
        {
            string res = GlobalFunctions.RIGHT("ola adeus", 5);
            Assert.AreEqual("adeus", res);
            res = GlobalFunctions.RIGHT("ola adeus", 30);
            Assert.AreEqual("ola adeus", res);
            res = GlobalFunctions.RIGHT("ola adeus", -1);
            Assert.AreEqual("", res);
            res = GlobalFunctions.RIGHT(null, 10);
            Assert.AreEqual("", res);
        }

        [Test]
        public void TestSubString()
        {
            string res = GlobalFunctions.SubString("ola adeus", 2 , 3);
            Assert.AreEqual("a a", res);
            res = GlobalFunctions.SubString("ola adeus", 4, 30);
            Assert.AreEqual("adeus", res);
            res = GlobalFunctions.SubString("ola adeus", -1, 2);
            Assert.AreEqual("", res);
            res = GlobalFunctions.SubString("ola adeus", 2, -1);
            Assert.AreEqual("", res);
            res = GlobalFunctions.SubString("ola adeus", 30, 2);
            Assert.AreEqual("", res);
            res = GlobalFunctions.SubString(null, 1, 1);
            Assert.AreEqual("", res);

        }

        [Test]
        public void TestRoundQG()
        {
            //casos especiais da function
            tRoundQG(0.0m, 0, 0);
            tRoundQG(0.50m, 0, 1);
            tRoundQG(0.9999999m, 0, 1);
            tRoundQG(0.499m, 0, 1);
            tRoundQG(0.489m, 0, 0);

            //0
            //aproximação a pares
            tRoundQG(12346.5m, 0, 12347);
            tRoundQG(12346.4m, 0, 12346);
            tRoundQG(12346.49m, 0, 12346);            
            tRoundQG(12346.498m, 0, 12346);
            tRoundQG(12346.498999999m, 0, 12346);
            tRoundQG(12346.499m, 0, 12347);

            //aproximação a impares
            tRoundQG(12343.5m, 0, 12344);
            tRoundQG(12343.4m, 0, 12343);
            tRoundQG(12343.49m, 0, 12343);            
            tRoundQG(12343.498m, 0, 12343);
            tRoundQG(12343.498999999m, 0, 12343);
            tRoundQG(12343.499m, 0, 12344);

            //1
            //aproximação a pares
            tRoundQG(12345.65m, 1, 12345.7m);
            tRoundQG(12345.64m, 1, 12345.6m);
            tRoundQG(12345.649m, 1, 12345.6m);            
            tRoundQG(12345.6498m, 1, 12345.6m);
            tRoundQG(12345.6498999999m, 1, 12345.6m);
            tRoundQG(12345.6499m, 1, 12345.7m);

            //aproximação a impares
            tRoundQG(12345.55m, 1, 12345.6m);
            tRoundQG(12345.54m, 1, 12345.5m);
            tRoundQG(12345.549m, 1, 12345.5m);            
            tRoundQG(12345.5498m, 1, 12345.5m);
            tRoundQG(12345.5498999999m, 1, 12345.5m);
            tRoundQG(12345.5499m, 1, 12345.6m);

            //2, negativos
            //aproximação a pares
            tRoundQG(-12345.065m, 2, -12345.07m);
            tRoundQG(-12345.064m, 2, -12345.06m);
            tRoundQG(-12345.0649m, 2, -12345.06m);            
            tRoundQG(-12345.06498m, 2, -12345.06m);
            tRoundQG(-12345.06498999999m, 2, -12345.06m);
            tRoundQG(-12345.06499m, 2, -12345.07m);

            //aproximação a impares
            tRoundQG(-12345.055m, 2, -12345.06m);
            tRoundQG(-12345.054m, 2, -12345.05m);
            tRoundQG(-12345.0549m, 2, -12345.05m);            
            tRoundQG(-12345.05498m, 2, -12345.05m);
            tRoundQG(-12345.05498999999m, 2, -12345.05m);
            tRoundQG(-12345.05499m, 2, -12345.06m);

        }

        private void tRoundQG(decimal n, int c, decimal expected)
        {
            decimal res;
            res = GlobalFunctions.RoundQG(n, c);
            System.Diagnostics.Debug.WriteLine("orig=" + n + " prec=" + c + " res=" + res);
            Assert.AreEqual(expected, res);
        }

        [Test]
        public void TestRound()
        {
            //casos especiais da function
            tRound(0.0m, 0, 0);
            tRound(0.5000001m, 0, 1);
            tRound(0.50m, 0, 1);
            tRound(0.4999999m, 0, 0);

            //0
            //aproximação a pares
            tRound(12346.5m, 0, 12347);
            tRound(12346.4m, 0, 12346);
            tRound(12346.49m, 0, 12346);
            tRound(12346.4999999m, 0, 12346);

            //aproximação a impares
            tRound(12343.5m, 0, 12344);
            tRound(12343.4m, 0, 12343);
            tRound(12343.49m, 0, 12343);
            tRound(12343.4999999m, 0, 12343);

            //1
            //aproximação a pares
            tRound(12345.65m, 1, 12345.7m);
            tRound(12345.64m, 1, 12345.6m);
            tRound(12345.649m, 1, 12345.6m);
            tRound(12345.649999999m, 1, 12345.6m);

            //aproximação a impares
            tRound(12345.55m, 1, 12345.6m);
            tRound(12345.54m, 1, 12345.5m);
            tRound(12345.549m, 1, 12345.5m);
            tRound(12345.54999999m, 1, 12345.5m);

            //2, negativos
            //aproximação a pares
            tRound(-12345.065m, 2, -12345.07m);
            tRound(-12345.064m, 2, -12345.06m);
            tRound(-12345.0649m, 2, -12345.06m);
            tRound(-12345.064999999m, 2, -12345.06m);

            //aproximação a impares
            tRound(-12345.055m, 2, -12345.06m);
            tRound(-12345.054m, 2, -12345.05m);
            tRound(-12345.0549m, 2, -12345.05m);
            tRound(-12345.054999999m, 2, -12345.05m);
        }

        private void tRound(decimal n, int c, decimal expected)
        {
            decimal res;
            res = GlobalFunctions.Round(n, c);
            Assert.AreEqual(expected, res);
        }

        [Test]
        public void TestDiferenca_entre_Datas()
        {
            decimal res;
            res = GlobalFunctions.Diferenca_entre_Datas(DateTime.MinValue, new DateTime(2010, 12, 01), "D");
            Assert.AreEqual(0, res);
            res = GlobalFunctions.Diferenca_entre_Datas(new DateTime(2010, 12, 01), DateTime.MinValue, "D");
            Assert.AreEqual(0, res);

            res = GlobalFunctions.Diferenca_entre_Datas(new DateTime(2010, 12, 01), new DateTime(2010, 12, 01), "D");
            Assert.AreEqual(0, res);
            res = GlobalFunctions.Diferenca_entre_Datas(new DateTime(2010, 11, 30), new DateTime(2010, 12, 01, 00, 02, 00), "D");
            Assert.AreEqual(1, res);
            res = GlobalFunctions.Diferenca_entre_Datas(new DateTime(2010, 12, 01), new DateTime(2010, 11, 20), "D");
            Assert.AreEqual(-11, res);

            res = GlobalFunctions.Diferenca_entre_Datas(new DateTime(2010,11,30), new DateTime(2010,12,01), "M");
            Assert.AreEqual(60 * 24, res);
            res = GlobalFunctions.Diferenca_entre_Datas(new DateTime(2010,11,30), new DateTime(2010,12,01, 01,11,02), "M");
            Assert.AreEqual(60 * 24 + 1 * 60 + 11, res);
            res = GlobalFunctions.Diferenca_entre_Datas(new DateTime(2010,12,01), new DateTime(2010,11,30, 00,00,04), "M");
            Assert.AreEqual(-60 * 24, res);

            res = GlobalFunctions.Diferenca_entre_Datas(new DateTime(2010,11,30), new DateTime(2010,12,01), "H");
            Assert.AreEqual(24, res);
            res = GlobalFunctions.Diferenca_entre_Datas(new DateTime(2010,11,30), new DateTime(2010,12,01, 01,11,02), "H");
            Assert.AreEqual(24 + 1, res);
            res = GlobalFunctions.Diferenca_entre_Datas(new DateTime(2010,12,01), new DateTime(2010,11,30, 00,00,04), "H");
            Assert.AreEqual(-24, res);

            res = GlobalFunctions.Diferenca_entre_Datas(new DateTime(2010,11,30), new DateTime(2010,12,01, 01,11,02), "S");
            Assert.AreEqual(60 * 60 * 24 + 1 * 60 * 60 + 60 * 11 + 2, res);
        }

    }
}

