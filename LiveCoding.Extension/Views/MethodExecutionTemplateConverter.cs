using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using LiveCoding.Extension.ViewModels;

namespace LiveCoding.Extension.Views
{
	internal sealed class MethodExecutionTemplateConverter : IValueConverter
	{
		public DataTemplate ReadyToExecuteTemplate { get; set; }

		public DataTemplate ExecutingTemplate { get; set; }

		public DataTemplate ExecutedTemplate { get; set; }

		public DataTemplate FailedTemplate { get; set; }

		public DataTemplate CanceledTemplate { get; set; }

		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			MethodExecutionState state = (MethodExecutionState)value;

			switch ( state )
			{
				case MethodExecutionState.ReadyToExecute:
					{
						return ReadyToExecuteTemplate;
					}
				case MethodExecutionState.Executing:
					{
						return ExecutingTemplate;
					}
				case MethodExecutionState.Executed:
					{
						return ExecutedTemplate;
					}
				case MethodExecutionState.Failed:
					{
						return FailedTemplate;
					}
				case MethodExecutionState.Canceled:
					{
						return CanceledTemplate;
					}
				default:
					{
						throw new ArgumentOutOfRangeException();
					}
			}
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			throw new NotSupportedException();
		}
	}
}