using CSGenio.business;
using CSGenio.framework;
using CSGenio.persistence;
using DocumentEngine;
using System;
using System.Collections;
using System.Xml.XPath;

namespace GenioServer.business
{
    class DocumentEngine
    {
        private Engine m_engine;
        private PersistentSupport m_sp;
        private User m_user;
        private Hashtable m_dados;

        public DocumentEngine(PersistentSupport sp, User user, Hashtable dados)
        {
            m_engine = new Engine();
            m_sp = sp;
            m_user = user;
            m_dados = dados;
        }

        public String GenerateDocument(String templatePath, String outname, String typeGen)
        {
            String lpTempPathBuffer = AppDomain.CurrentDomain.BaseDirectory + "\\temp\\";
            DateTime ct = DateTime.Now;
            outname = outname.Replace("@id", ct.ToString("yyyy%M%dT%h%m%s"));
            String outputPath = lpTempPathBuffer + outname;

            return GenerateDocument(templatePath, outputPath, outname, typeGen);
        }

        public String GenerateDocument(String templatePath, String outputPath, String outname, String typeGen)
        {
            m_engine.GenerationType = typeGen;
            m_engine.TemplatePath = templatePath;
            m_engine.OutputPath = outputPath;
            m_engine.Lang = m_user.Language;

            String[] tags = m_engine.getTagsFromTemplate();
            Hashtable manualQueries = PersistentSupport.getManualQueries();

            foreach (String tag in tags)
            {
                if (manualQueries.ContainsKey(tag))
                {
                    ManualQuery query = (ManualQuery)manualQueries[tag];
                    readXml(query);
                    query.setParams(m_dados);
                    DataMatrix matrix = query.Run(m_sp);
                    FillTagResults(tag, query, matrix);
                }
            }

            m_engine.generate();

            return outputPath;
        }

        public void FillTagResults(String tag, ManualQuery query, DataMatrix matrix)
        {
            String sepRows = query.SeparadorLinha;
            String sepColumn = query.SeparadorColuna;
            int resultType = Int32.Parse(query.TipoResultado);
            String ignoreEmpty = query.IgnorarResultadosVazios;

            m_engine.setRowsColumns(tag, matrix.NumRows, matrix.NumCols);

// USE /[MANUAL PRO FILLTAGS1]/


            if (matrix.NumRows == 0)
            {
                m_engine.setRowsColumns(tag, 1, 1);
                m_engine.addResults(tag, 0, 0, "", resultType);
            }

// USE /[MANUAL PRO FILLTAGS2]/


            m_engine.setEmptyResults(tag, ignoreEmpty);

            for (int r = 0; r < matrix.NumRows; r++)
                for (int c = 0; c < matrix.NumCols; c++)
                {
                    object cv = matrix.GetDirect(r, c);

                    String res = "";
                    //pensar em criar uma classe de conversao to este tipo de relatórios
                    if (cv is DateTime)
                        res = ((DateTime)cv).ToString().Replace(" 00:00:00", "").Replace("-", "/");
                    else
                        res = cv.ToString();

                    if (cv is byte[])
                        m_engine.addResults(tag, r, c, matrix.GetBinary(r, c), resultType);
                    else
                        m_engine.addResults(tag, r, c, res, resultType);
                    m_engine.setSeparators(tag, sepRows, sepColumn);
                }
        }

        public void readXml(ManualQuery query)
        {
            try
            {
                String tag = query.Id;
                String xmlFileName = AppDomain.CurrentDomain.BaseDirectory + "//PROqueries.xml";
                // create an XPathDocument object
                XPathDocument xmlPathDoc = new XPathDocument(xmlFileName);

                if (xmlPathDoc != null)
                {
                    // create a navigator for the xpath doc
                    XPathNavigator xNav = xmlPathDoc.CreateNavigator();
                    //run the XPath query
                    XPathNodeIterator xPathIt = xNav.Select("/infotags/queries/queryinfo[@id='" + tag + "']");

                    //use the XPathNodeIterator to display the results
                    if (xPathIt.Count > 0)
                    {
                        //begin to loop through the titles and begin to display them
                        while (xPathIt.MoveNext())
                        {
                            xPathIt.Current.MoveToChild("query", String.Empty);
                            query.Query = xPathIt.Current.Value;

                            xPathIt.Current.MoveToParent();
                            String resultType = xPathIt.Current.GetAttribute("resulttype", String.Empty);
                            query.TipoResultado = resultType;

                            String ignoreEmpty = xPathIt.Current.GetAttribute("ignoreempty", String.Empty);
                            query.IgnorarResultadosVazios = ignoreEmpty;

                            //separadores de colunas e linhas
                            if (resultType.Equals("2"))
                            {

                                xPathIt.Current.MoveToChild("separators", String.Empty);
                                xPathIt.Current.MoveToChild("separatorcolumn", String.Empty);
                                query.SeparadorColuna = xPathIt.Current.Value;
                                xPathIt.Current.MoveToParent();

                                xPathIt.Current.MoveToChild("separatorrow", String.Empty);
                                query.SeparadorLinha = xPathIt.Current.Value;
                                xPathIt.Current.MoveToParent();
                                xPathIt.Current.MoveToParent();

                            }
                        }
                    }
                }
            }
            catch (Exception) { }
        }
    }
}
