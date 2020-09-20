using System;
using System.Collections;
using Xamarin.Forms;

namespace ParkHyderabadOperator.CustomXamarinElementsModel
{
    public class AlternateColorDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate EvenTemplate { get; set; }
        public DataTemplate UnevenTemplate { get; set; }


        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            ListView lv = container as ListView;
            if (lv != null)
            {
                try
                {
                    IList listItem = lv.ItemsSource as IList;

                    int idx = listItem.IndexOf(item);
                    return idx % 2 == 0 ? EvenTemplate : UnevenTemplate;
                }
                catch (Exception ex)
                {
                    return UnevenTemplate;
                }
            }
            else
            {
                return UnevenTemplate;
            }

        }
    }
}

