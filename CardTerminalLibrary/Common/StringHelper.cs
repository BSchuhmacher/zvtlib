namespace Wiffzack.Devices.CardTerminals
{
	/**
	 * This class is contains String methodes used by the library.
	 */
	public class StringHelper
	{
		public StringHelper ()
		{
		}
		/**
		 * The methode adds blanks after ever capital letter of a string.
		 */
		public static string addSpaces(string before){
			string after="";
			if(before==null)
				return null;
			if(before.Length==0)
				return after;
			char[] beforearr=before.ToCharArray();
			after=""+beforearr[0];
			for(int i=1;i<beforearr.Length;i++){
				if(char.IsUpper(beforearr[i])){
					after=after+" "+beforearr[i];
				}else{
					after=after+beforearr[i];
				}
			}
			return after;
		}
	}
}

