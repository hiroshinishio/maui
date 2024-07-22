﻿using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Hosting;
using Xunit;

namespace Microsoft.Maui.DeviceTests
{
	[Category(TestCategory.HybridWebView)]
	public partial class HybridWebViewTests : ControlsHandlerTestBase
	{
		void SetupBuilder()
		{
			EnsureHandlerCreated(builder =>
			{
				builder.ConfigureMauiHandlers(handlers =>
				{
					handlers.AddHandler<HybridWebView, HybridWebViewHandler>();
				});
			});
		}

		[Fact]
		public async Task LoadsHtmlAndSendReceiveRawMessage()
		{
#if ANDROID
			// NOTE: skip this test on older Android devices because it is not currently supported on these versions
			if (!System.OperatingSystem.IsAndroidVersionAtLeast(24))
			{
				return;
			}
#endif

			SetupBuilder();

			await InvokeOnMainThreadAsync(async () =>
			{
				var hybridWebView = new HybridWebView()
				{
					WidthRequest = 100,
					HeightRequest = 100,

					HybridRoot = "HybridTestRoot",
				};

				var lastRawMessage = "";

				hybridWebView.RawMessageReceived += (s, e) =>
				{
					lastRawMessage = e.Message;
				};

				var handler = CreateHandler(hybridWebView);

				var platformView = handler.PlatformView;

				// Set up the view to be displayed/parented and run our tests on it
				await AttachAndRun(hybridWebView, async (handler) =>
				{
					await WaitForHybridWebViewLoaded(hybridWebView);

					const string TestRawMessage = "Hybrid\"\"'' {Test} with chars!";
					hybridWebView.SendRawMessage(TestRawMessage);

					var passed = false;

					for (var i = 0; i < 10; i++)
					{
						if (lastRawMessage == "You said: " + TestRawMessage)
						{
							passed = true;
							break;
						}

						await Task.Delay(1000);
					}

					Assert.True(passed, $"Waited for raw message response but it never arrived or didn't match (last message: {lastRawMessage})");
				});
			});
		}

		private async Task WaitForHybridWebViewLoaded(HybridWebView hybridWebView)
		{
			const int NumRetries = 10;
			const int RetryDelay = 500;
			for (var i = 0; i < NumRetries; i++)
			{
				// 1. Check that the window.HybridWebView object exists (as set by HybridWebView.js)
				// 2. Check that the test page's HTML is loaded by checking for the HTML element defined in the Resources\Raw\HybridTestRoot\index.html test page
				var loaded = await hybridWebView.EvaluateJavaScriptAsync("Object.hasOwn(window, 'HybridWebView') && (document.getElementById('htmlLoaded') !== null)");
				if (loaded == "true")
				{
					return;
				}

				await Task.Delay(RetryDelay);
			}

			Assert.Fail($"Waited {NumRetries * RetryDelay:N0}ms for the HybridWebView test page to be ready, but it wasn't.");
		}

		[Fact]
		public async Task CallJavaScriptMethodsAndGetResults()
		{
#if ANDROID
			// NOTE: skip this test on older Android devices because it is not currently supported on these versions
			if (!System.OperatingSystem.IsAndroidVersionAtLeast(24))
			{
				return;
			}
#endif

			SetupBuilder();

			await InvokeOnMainThreadAsync(async () =>
			{
				var hybridWebView = new HybridWebView()
				{
					WidthRequest = 100,
					HeightRequest = 100,

					HybridRoot = "HybridTestRoot",
				};

				var handler = CreateHandler(hybridWebView);

				var platformView = handler.PlatformView;

				// Set up the view to be displayed/parented and run our tests on it
				await AttachAndRun(hybridWebView, async (handler) =>
				{
					await WaitForHybridWebViewLoaded(hybridWebView);

					// Simple method call and return value

					var x1 = 123.456m;
					var y1 = 654.321m;

					var result1 = await hybridWebView.InvokeJavaScriptAsync<ComputationResult>(
						"AddNumbers",
						ComputationResultContext.Default.ComputationResult,
						new object[] { x1, y1 },
						new[] { ComputationResultContext.Default.Decimal, ComputationResultContext.Default.Decimal });

					Assert.NotNull(result1);
					Assert.Equal(777.777m, result1.result);
					Assert.Equal("Addition", result1.operationName);


					// Method call with nulls and return value

					var x2 = 123.456m;
					var y2 = 654.321m;

					var result2 = await hybridWebView.InvokeJavaScriptAsync<ComputationResult>(
						"AddNumbersWithNulls",
						ComputationResultContext.Default.ComputationResult,
						new object[] { x2, null, y2, null },
						new[] { ComputationResultContext.Default.Decimal, null, ComputationResultContext.Default.Decimal, null });

					Assert.NotNull(result2);
					Assert.Equal(777.777m, result2.result);
					Assert.Equal("AdditionWithNulls", result2.operationName);
				});
			});
		}

		[Fact]
		public async Task EvaluateJavaScriptAndGetResult()
		{
#if ANDROID
			// NOTE: skip this test on older Android devices because it is not currently supported on these versions
			if (!System.OperatingSystem.IsAndroidVersionAtLeast(24))
			{
				return;
			}
#endif

			SetupBuilder();

			await InvokeOnMainThreadAsync(async () =>
			{
				var hybridWebView = new HybridWebView()
				{
					WidthRequest = 100,
					HeightRequest = 100,

					HybridRoot = "HybridTestRoot",
				};

				var handler = CreateHandler(hybridWebView);

				var platformView = handler.PlatformView;

				// Set up the view to be displayed/parented and run our tests on it
				await AttachAndRun(hybridWebView, async (handler) =>
				{
					await WaitForHybridWebViewLoaded(hybridWebView);

					// Run some JavaScript and get result
					var result1 = await hybridWebView.EvaluateJavaScriptAsync("EvaluateMeWithParamsAndReturn('abc', 'def')");
					Assert.Equal("abcdef", result1);


					// Method call with nulls and return value
					var result2 = await hybridWebView.EvaluateJavaScriptAsync("EvaluateMeWithParamsAndAsyncReturn('abc', 'def')");
					Assert.Equal("abcdef", result2);
				});
			});
		}

		public class ComputationResult
		{
			public decimal result { get; set; }
			public string operationName { get; set; }
		}

		[JsonSourceGenerationOptions(WriteIndented = true)]
		[JsonSerializable(typeof(ComputationResult))]
		[JsonSerializable(typeof(decimal))]
		[JsonSerializable(typeof(string))]
		internal partial class ComputationResultContext : JsonSerializerContext
		{
		}
	}
}
