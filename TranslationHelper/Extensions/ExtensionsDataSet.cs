using System.Data;

namespace TranslationHelper.Extensions
{
    static class ExtensionsDataSet
    {
        /// <summary>
        /// Get overall count of rows in al tables of the <paramref name="dataSet"/>
        /// </summary>
        /// <param name="dataSet">input dataset where calculate count</param>
        /// <returns>Overall count of rows in all tables of the <paramref name="dataSet"/></returns>
        public static int GetRowsCount(this DataSet dataSet)
        {
            int resultRowsCount = 0;
            foreach (DataTable dataTable in dataSet.Tables)
            {
                foreach (var dataRow in dataTable.Rows)
                {
                    resultRowsCount++;
                }
            }

            return resultRowsCount;
        }
    }
}
