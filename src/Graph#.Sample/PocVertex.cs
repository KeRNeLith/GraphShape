using System.Diagnostics;
namespace GraphSharp.Sample
{
	/// <summary>
	/// A simple identifiable vertex.
	/// </summary>
	[DebuggerDisplay( "{ID}" )]
	public class PocVertex
	{
		public string ID
		{
			get;
			private set;
		}

		public PocVertex( string id )
		{
			ID = id;
		}

        public override string ToString()
        {
            return ID;
        }
	}
}