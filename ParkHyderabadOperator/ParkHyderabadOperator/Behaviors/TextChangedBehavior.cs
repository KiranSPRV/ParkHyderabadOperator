using System;
using Xamarin.Forms;

namespace ParkHyderabadOperator.Behaviors
{
    public class TextChangedBehavior : Behavior<SearchBar>
    {
        protected override void OnAttachedTo(SearchBar bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.TextChanged += Bindable_TextChanged;
        }

        protected override void OnDetachingFrom(SearchBar bindable)
        {
            try
            {
                base.OnDetachingFrom(bindable);
                bindable.TextChanged -= Bindable_TextChanged;
            }
            catch (Exception ex)
            {

            }
        }

        private void Bindable_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                ((SearchBar)sender).Text = e.NewTextValue.ToUpper();
            }
            catch (Exception ex)
            {

            }

        }
    }
}
