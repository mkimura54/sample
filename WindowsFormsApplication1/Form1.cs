using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			var lockObj = new object();
			int result = 0;
			var beforeTime = DateTime.Now;
#if true
			var clt = new System.Threading.CancellationTokenSource();
			ParallelOptions opt = new ParallelOptions();
			opt.MaxDegreeOfParallelism = 5;
			opt.CancellationToken = clt.Token;
			try
			{
				Parallel.For(0, 10000, opt, n =>
				{
					lock (lockObj)
					{
						result = result + n;
						if (result > 10000)
						{
							clt.Cancel();
						}
					}
				});
			}
			catch
			{
				MessageBox.Show("canceled");
			}
#else
			for (int i = 0; i < 10000; i++)
			{
				result = result + i;
			}
#endif
			var afterTime = DateTime.Now;
			MessageBox.Show("finished:" + result + " # " + (afterTime - beforeTime).TotalMilliseconds);
		}

		private void button2_Click(object sender, EventArgs e)
		{
			var beforeTime = DateTime.Now;

			int result = 0;
			var data = new List<int>();
			for (int i = 0; i < 10000; i++)
			{
				data.Add(i);
			}

#if true
			result = data.AsParallel().Sum();
#else
			for (int i = 0; i < data.Count; i++)
			{
				result = result + i;
			}
#endif

			var afterTime = DateTime.Now;
			MessageBox.Show("finished:" + result + " # " + (afterTime - beforeTime).TotalMilliseconds);
		}

		private void button3_Click(object sender, EventArgs e)
		{
			// 重たい処理
			Task<DateTime> task = Task.Run(() =>
			{
				try
				{
					//throw new Exception();
					Thread.Sleep(5000);
					//MessageBox.Show("fin.");
				}
				catch
				{
					Console.WriteLine("catched exception internal");
				}
				return DateTime.Now;
			});

			{
				while (!task.IsCompleted)
				{
					Thread.Sleep(500);
					Console.WriteLine(task.Status);
				}
				//task.Wait();
				MessageBox.Show("test");
			}
			MessageBox.Show(task.Result.ToString());
			//MessageBox.Show(task.IsCanceled.ToString());
		}

		private async void button4_Click(object sender, EventArgs e)
		{
			button4.Text = "running";

			await Task.Run(() =>
			{
				Thread.Sleep(5000);
			});

			button4.Text = "finished";
		}
	}
}
