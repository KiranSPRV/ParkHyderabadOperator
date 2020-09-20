using System;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace ParkHyderabadOperator.Behaviors
{
    public class AlphaValidation : Behavior<Entry>
    {

        public int MaxLength { get; set; }
        public int MinLength { get; set; }

        const string alphanumericRegex = @"^[a-zA-Z.\s]*$";

        protected override void OnAttachedTo(Entry bindable)
        {
            bindable.TextChanged += HandleTextChanged;
            base.OnAttachedTo(bindable);
        }

        void HandleTextChanged(object sender, TextChangedEventArgs e)
        {
            ((Entry)sender).TextColor = Color.FromHex("#010101");
            bool IsValid = false;
            IsValid = (Regex.IsMatch(e.NewTextValue, alphanumericRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)));

            if (IsValid)
            {
                ((Entry)sender).Text = e.NewTextValue.ToUpper();
            }
            else
            {
                ((Entry)sender).Text = e.NewTextValue.Substring(0, e.NewTextValue.Length - 1).ToUpper();
            }
            //((Entry)sender).TextColor = IsValid ? Color.Default : Color.Red;
            if (e.NewTextValue.Length < this.MinLength)
            {
                ((Entry)sender).TextColor = Color.Red;
            }
            if (e.NewTextValue.Length > this.MaxLength)
            {
                string entryText = e.NewTextValue;
                entryText = entryText.Remove(entryText.Length - 1); // remove last char
                //((Entry)sender).Text = e.NewTextValue.Substring(0, MaxLength).ToUpper();
                ((Entry)sender).TextColor = Color.Red;
            }


        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= HandleTextChanged;
            base.OnDetachingFrom(bindable);
        }
    }
}
