using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GridPromocional.Extensions
{
    public static class ViewDataExtensions
    {
        public static void Put<T>(this ViewDataDictionary viewData, string key, T value) where T : class
        {
            viewData[key] = value;
        }

        public static T? Get<T>(this ViewDataDictionary viewData, string key) where T : class
        {
            viewData.TryGetValue(key, out object? o);
            return (T?)o;
        }

        public static List<T> GetList<T>(this ViewDataDictionary viewData, string key) where T : class
        {
            return Get<List<T>>(viewData, key) ?? new();
        }

        public static void PutListItem<T>(this ViewDataDictionary viewData, string key, T value) where T : class
        {
            List<T> list = GetList<T>(viewData, key);
            list.Add(value);
            viewData[key] = list;
        }
    }
}