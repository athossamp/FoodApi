using System.Data.Common;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;

namespace log_food_api
{
    public static class RequestExtensions
    {
        public static async Task<string> ReadAsStringAsync(this Stream requestBody, bool leaveOpen = false)
        {
            using StreamReader reader = new(requestBody, leaveOpen: leaveOpen);
            var bodyAsString = await reader.ReadToEndAsync();

            return bodyAsString;
        }
    }
    public static class Utils
    {  
        public static List<T> FillList<T>(this DbCommand cmd)
        {
            List<T> list = new List<T>();
            bool lopen = false;

            try
            {
                if (cmd.Connection.State != System.Data.ConnectionState.Open)
                {
                    lopen = true;
                    cmd.Connection.Open();
                }

                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        T item = (T)Activator.CreateInstance(typeof(T));

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            var prop = item.GetType().GetProperty(reader.GetName(i), BindingFlags.Public | BindingFlags.Instance);

                            if (prop == null)
                            {
                                continue;
                            }

                            if (reader[i] == DBNull.Value)
                            {
                                continue;
                            }

                            prop.SetValue(item, reader[i]);
                        }

                        list.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (lopen)
                {
                    cmd.Connection.Close();
                }
            }

            return list;
        }
        public static string FillToJson<T>(this DbCommand cmd)
        {
            List<T> list = new List<T>();

            try
            {
                cmd.Connection.Open();

                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        T item = (T)Activator.CreateInstance(typeof(T));

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            var prop = item.GetType().GetProperty(reader.GetName(i), BindingFlags.Public | BindingFlags.Instance);

                            if (prop == null)
                            {
                                continue;
                            }

                            prop.SetValue(item, reader[i]);
                        }

                        list.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Connection.Close();
            }

            return JsonSerializer.Serialize(list);
        }
    }
}