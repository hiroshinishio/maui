﻿using System;
using Android.App;
using Android.Content.Res;

namespace Microsoft.Maui
{
	public static class DatePickerExtensions
	{
		static readonly int[][] ColorStates =
		{
			new[] { Android.Resource.Attribute.StateEnabled },
			new[] { -Android.Resource.Attribute.StateEnabled }
		};

		public static void UpdateFormat(this MauiDatePicker nativeDatePicker, IDatePicker datePicker)
		{
			nativeDatePicker.SetText(datePicker);
		}

		public static void UpdateDate(this MauiDatePicker nativeDatePicker, IDatePicker datePicker)
		{
			nativeDatePicker.SetText(datePicker);
		}

		public static void UpdateTextColor(this MauiDatePicker nativeDatePicker, IDatePicker datePicker)
		{
			nativeDatePicker.UpdateTextColor(datePicker, null);
		}

		public static void UpdateTextColor(this MauiDatePicker nativeDatePicker, IDatePicker datePicker, ColorStateList? defaultTextColor)
		{
			var textColor = datePicker.TextColor;

			if (textColor == null)
			{
				if (defaultTextColor != null)
					nativeDatePicker.SetTextColor(defaultTextColor);
			}
			else
			{
				var androidColor = textColor.ToNative();

				if (!nativeDatePicker.TextColors.IsOneColor(ColorStates, androidColor))
				{
					var acolor = androidColor.ToArgb();
					nativeDatePicker.SetTextColor(new ColorStateList(ColorStates, new[] { acolor, acolor }));
				}
			}
		}

		public static void UpdateMinimumDate(this MauiDatePicker nativeDatePicker, IDatePicker datePicker)
		{
			nativeDatePicker.UpdateMinimumDate(datePicker, null);
		}

		public static void UpdateMinimumDate(this MauiDatePicker nativeDatePicker, IDatePicker datePicker, DatePickerDialog? datePickerDialog)
		{
			if (datePickerDialog != null)
			{
				datePickerDialog.DatePicker.MinDate = (long)datePicker.MinimumDate.ToUniversalTime().Subtract(DateTime.MinValue.AddYears(1969)).TotalMilliseconds;
			}
		}

		public static void UpdateMaximumDate(this MauiDatePicker nativeDatePicker, IDatePicker datePicker)
		{
			nativeDatePicker.UpdateMinimumDate(datePicker, null);
		}

		public static void UpdateMaximumDate(this MauiDatePicker nativeDatePicker, IDatePicker datePicker, DatePickerDialog? datePickerDialog)
		{
			if (datePickerDialog != null)
			{
				datePickerDialog.DatePicker.MaxDate = (long)datePicker.MaximumDate.ToUniversalTime().Subtract(DateTime.MinValue.AddYears(1969)).TotalMilliseconds;
			}
		}

		internal static void SetText(this MauiDatePicker nativeDatePicker, IDatePicker datePicker)
		{
			nativeDatePicker.Text = datePicker.Date.ToString(datePicker.Format);
		}
	}
}