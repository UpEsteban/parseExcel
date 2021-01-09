using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Sedical.DAO;
using CsvHelper;
using System.Data;
using Newtonsoft.Json;
using System.Globalization;

namespace Sedical
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("SEDICAL CONCENTRADOR DE CONTADORES");

            Console.WriteLine("El archivo debe estar en formato valido JSON.");
            Console.WriteLine("Dime el nombre del archivo sin formato: ");
            string namePath = Console.ReadLine();

            var jsonString = File.ReadAllText(namePath + ".json");

            var obj = System.Text.Json.JsonSerializer.Deserialize<List<SedicalDAO>>(jsonString);

            var values = obj.Where(x => x.name.Equals("ValueDesc")).FirstOrDefault();

            var result = new List<SedicalO>();

            var lastValue = values.columns.Where(x => x.name.Equals("LoggerLastValue")).FirstOrDefault();
            var name = obj.Where(x => x.name.Equals("Device")).FirstOrDefault().columns.Where(x => x.name.Equals("Name")).FirstOrDefault();

            if (values != null)
            {
                var indice = 20;
                var name_ind = 6;
                do
                {
                    result.Add(new SedicalO
                    {
                        Id = lastValue.values[indice].ToString(),
                        Name = name.values[name_ind].ToString(),
                        LecturaEnergia = lastValue.values[indice - 14].ToString(),
                    });
                    indice += 96;
                    name_ind++;
                } while (lastValue.values.Count() >= indice);

                var jsonstring = System.Text.Json.JsonSerializer.Serialize(result);
                jsonToCSV(jsonstring, " ");
            }
            Console.WriteLine("Pulsa cualquier tecla para finalizar.");
            Console.WriteLine("FINIQUITADO!");
            Console.ReadLine();
        }

        public static void jsonToCSV(string jsonContent, string delimiter)
        {
            using (var writer = new StreamWriter("sedical_result.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                //csv.Configuration.SkipEmptyRecords = true;
                //csv.Configuration.WillThrowOnMissingField = false;
                csv.Configuration.Delimiter = delimiter;

                using (var dt = jsonStringToTable(jsonContent))
                {
                    foreach (DataColumn column in dt.Columns)
                    {
                        csv.WriteField(column.ColumnName);
                    }
                    csv.NextRecord();

                    foreach (DataRow row in dt.Rows)
                    {
                        for (var i = 0; i < dt.Columns.Count; i++)
                        {
                            csv.WriteField(row[i]);
                        }
                        csv.NextRecord();
                    }
                }
            }
        }
        public static DataTable jsonStringToTable(string jsonContent)
        {
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(jsonContent);
            return dt;
        }
    }
}
