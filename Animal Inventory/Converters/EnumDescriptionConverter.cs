using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Data;

namespace Animal_Inventory.Converters
{
    public class EnumDescriptionConverter : IValueConverter
    {
        public string[] Strings => GetStrings();
        Dictionary<string, (MVVM.ViewModels.AnimalType Type, int Order)> cachedEnums = new Dictionary<string, (MVVM.ViewModels.AnimalType Type, int Order)>();

        public string[] GetStrings()
        {
            foreach (MVVM.ViewModels.AnimalType type in Enum.GetValues(typeof(MVVM.ViewModels.AnimalType)))
                GetEnumDescription(type);

            return cachedEnums.OrderBy(x => x.Value.Order).Select(x => x.Key).ToArray();
        }
        private string GetEnumDescription(Enum enumObj)
        {
            FieldInfo fieldInfo = enumObj.GetType().GetField(enumObj.ToString());
            object[] attribArray = fieldInfo.GetCustomAttributes(false);
            int order = 0;

            string description = string.Empty;
            if (attribArray.Length == 0)
                description = enumObj.ToString();
            else
            {
                DescriptionAttribute attrib = null;

                foreach (var att in attribArray)
                {
                    if (att is DescriptionAttribute)
                        attrib = att as DescriptionAttribute;
                    else if (att is System.ComponentModel.DataAnnotations.DisplayAttribute displayAttribute)
                        order = displayAttribute.Order;
                }


                if (attrib != null)
                    description = attrib.Description;
            }

            if (!cachedEnums.ContainsKey(description))
                cachedEnums.Add(description, ((MVVM.ViewModels.AnimalType)enumObj, order));

            return description;

        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Enum myEnum = (Enum)value;
            string description = GetEnumDescription(myEnum);
            return description;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return MVVM.ViewModels.AnimalType.None;

            cachedEnums.TryGetValue(value.ToString(), out (MVVM.ViewModels.AnimalType Type, int Order) animalInfo);
            return animalInfo.Type;
        }
    }
}
