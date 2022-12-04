using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CanvasComponent.Model
{
    public class NotifyPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new(name));

        protected void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
            => PropertyChanged?.Invoke(sender, e);

        protected void ChangeAndRaise<T>(ref T field, T value, [CallerMemberName] string name = null)
        {
            if (field is null || !field.Equals(value))
            {
                field = value;
                OnPropertyChanged(name);
            }
        }

        protected void ChangeAndRaise<T, V>(Expression<Func<V, T>> expr, V field, T value, [CallerMemberName] string name = null)
        {
            var member = ((MemberExpression)expr.Body).Member as PropertyInfo;
            member.SetValue(field, value);
            OnPropertyChanged(name);
        }

    }
}
