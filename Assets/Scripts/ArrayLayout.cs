[System.Serializable]
public class ArrayLayout
{
	[System.Serializable]
	public struct rowData
	{
		public bool[] row;
	}

	public rowData[] rows = new rowData[7]; // Initialize with 7 rows if boardHeight is 7
}
