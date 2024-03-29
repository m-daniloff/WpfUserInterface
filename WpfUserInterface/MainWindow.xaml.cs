﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfUserInterface
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void executeSync_Click(object sender, RoutedEventArgs e)
		{
			var watch = System.Diagnostics.Stopwatch.StartNew();

			RunDownloadSync();

			watch.Stop();
			var elapsedMs = watch.ElapsedMilliseconds;

			resultsWindow.Text += $"Total execution time: { elapsedMs }";
		}

		private async void executeAsync_Click(object sender, RoutedEventArgs e)
		{
			var watch = System.Diagnostics.Stopwatch.StartNew();

			await RunDownloadParallelAsync();

			watch.Stop();
			var elapsedMs = watch.ElapsedMilliseconds;

			resultsWindow.Text += $"Total execution time: { elapsedMs }";
		}

		private List<string> PrepData()
		{
			var output = new List<string>();
			resultsWindow.Text = string.Empty;

			output.Add("https://www.yahoo.com");
			output.Add("https://www.google.com");
			output.Add("https://www.microsoft.com");
			output.Add("https://www.cnn.com");
			output.Add("https://www.codeproject.com");
			output.Add("https://www.stackoverflow.com");

			return output;
		}

		private void RunDownloadSync()
		{
			var websites = PrepData();
			foreach(string site in websites)
			{
				WebsiteDataModel results = DownloadWebsite(site);
				ReportWebsiteInfo(results);
			}
		}

		private WebsiteDataModel DownloadWebsite(string websiteUrl)
		{
			var output = new WebsiteDataModel();
			WebClient client = new WebClient();

			output.WebsiteUrl = websiteUrl;
			output.WebsiteData = client.DownloadString(websiteUrl);

			return output;

		}

		private async Task<WebsiteDataModel> DownloadWebsiteAsync(string websiteUrl)
		{
			var output = new WebsiteDataModel();
			WebClient client = new WebClient();

			output.WebsiteUrl = websiteUrl;
			output.WebsiteData = await client.DownloadStringTaskAsync(websiteUrl);

			return output;

		}

		private void ReportWebsiteInfo(WebsiteDataModel data)
		{
			resultsWindow.Text += $" { data.WebsiteUrl } downloaded: {data.WebsiteData.Length } characters long. { Environment.NewLine }";
		}

		private async Task RunDownloadAsync()
		{
			var websites = PrepData();
			foreach (string site in websites)
			{
				WebsiteDataModel results = await Task.Run (() => DownloadWebsite(site));
				ReportWebsiteInfo(results);
			}
		}

		private async Task RunDownloadParallelAsync()
		{
			var websites = PrepData();
			List<Task<WebsiteDataModel>> tasks = new List<Task<WebsiteDataModel>>();
			foreach (string site in websites)
			{
				//tasks.Add( Task.Run(() => DownloadWebsite(site)));
				tasks.Add(DownloadWebsiteAsync(site));

			}

			var results = await Task.WhenAll(tasks);
			foreach(var item in results)
			{
				ReportWebsiteInfo(item);
			}
		}
	}
}
