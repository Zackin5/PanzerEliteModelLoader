namespace PanzerElite.Extensions
{
    public static class ArrayExtensions
    {
        public static int GetLargestValue(this int[,] array)
        {
            var largestValue = int.MinValue;

            for (var x = 0; x < array.GetLength(0); x++)
            {
                for (var y = 0; y < array.GetLength(1); y++)
                {
                    var arrayValue = array[x, y];

                    if (arrayValue > largestValue)
                        largestValue = arrayValue;
                }
            }

            return largestValue;
        }
    }
}
