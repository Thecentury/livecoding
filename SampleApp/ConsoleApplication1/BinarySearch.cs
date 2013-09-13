using System.Text.RegularExpressions;

namespace ConsoleApplication1
{
	class BinarySearch
	{
		public void PerformBinarySearch()
		{
			const int count = 100;

			int[] array = new int[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = i;
				string s = i.ToString();
			}

			const int needle = count / 2 - 1;
			int min = 0;
			int max = count;

			while (min <= max)
			{
				int mid = (max + min) / 2;
				int value = array[mid];
				if (value == needle)
				{
					break;
				}
				else if (value < needle)
				{
					min = mid + 1;
				}
				else
				{
					max = mid - 1;
				}
			}
		}

		public void SomeLoop()
		{
			for (int i = 0; i < 1; i++)
			{
				Regex r = new Regex(@"\d");
			}
		}
	}
}