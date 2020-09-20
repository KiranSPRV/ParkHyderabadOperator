using System;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace ParkHyderabadOperator.Behaviors
{
    public class NumberValidationBehavior : Behavior<Entry>
    {
        public int MaxLength { get; set; }
        public int MinLength { get; set; }

        const string numaricRegex = @"^[0-9]*$";


        protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(entry);
        }

        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(entry);
        }

        void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            int result;

            bool isValid = (Regex.IsMatch(args.NewTextValue, numaricRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250))); //int.TryParse(args.NewTextValue, out result);

            ((Entry)sender).TextColor = isValid ? Color.Default : Color.Red;

            if(!isValid)
            {
                string entryText = args.NewTextValue;
                ((Entry)sender).Text = args.NewTextValue.Substring(0, args.NewTextValue.Length-1).ToUpper();
            }

            if (args.NewTextValue.Length > this.MaxLength)
            {
                string entryText = args.NewTextValue;
                ((Entry)sender).Text = args.NewTextValue.Substring(0, MaxLength).ToUpper();
            }
            if (args.NewTextValue.Length < this.MinLength)
            {
                ((Entry)sender).TextColor = Color.Red;
            }

        }
    }
}
