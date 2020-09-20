using Xamarin.Forms;

namespace ParkHyderabadOperator.Behaviors
{
    public class MaxLengthValidatorBehavior : Behavior<Entry>
    {
        public static readonly BindableProperty MaxLengthProperty = BindableProperty.Create("MaxLength", typeof(int), typeof(MaxLengthValidatorBehavior), 0);
        public static readonly BindableProperty MinLengthProperty = BindableProperty.Create("MinLength", typeof(int), typeof(MaxLengthValidatorBehavior), 0);

        public int MaxLength
        {
            get { return (int)GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }
        public int MinLength
        {
            get { return (int)GetValue(MinLengthProperty); }
            set { SetValue(MinLengthProperty, value); }
        }
        protected override void OnAttachedTo(Entry bindable)
        {
            bindable.TextChanged += bindable_TextChanged;
        }

        private void bindable_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if ((e.NewTextValue.Length < MinLength) || (e.NewTextValue.Length >= MaxLength))
            //{
            //    ((Entry)sender).TextColor = Color.Red;
            //}
            if (e.NewTextValue.Length >= MaxLength)
                ((Entry)sender).Text = e.NewTextValue.Substring(0, MaxLength);



        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= bindable_TextChanged;

        }
    }
}
