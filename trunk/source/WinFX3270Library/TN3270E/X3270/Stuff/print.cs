#region License
/* 
 *
 * Open3270 - A C# implementation of the TN3270/TN3270E protocol
 *
 *   Copyright � 2004-2006 Michael Warriner. All rights reserved
 * 
 * This is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation; either version 2.1 of
 * the License, or (at your option) any later version.
 *
 * This software is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this software; if not, write to the Free
 * Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
 * 02110-1301 USA, or see the FSF site: http://www.fsf.org.
 */
#endregion
using System;
using System.IO;

namespace Open3270.TN3270
{
	/// <summary>
	/// Summary description for print.
	/// </summary>
	internal class Print:IDisposable
	{
		Telnet telnet;
		internal Print(Telnet telnet)
		{
			this.telnet = telnet;
		}
		/*
		 * Print the ASCIIfied contents of the screen onto a stream.
		 * Returns True if anything printed, False otherwise.
		 */
		bool fprint_screen(StreamWriter f, bool even_if_empty)
		{
			int i;
			byte e;
			byte c;
			int ns = 0;
			int nr = 0;
			bool any = false;
			byte fa = telnet.Controller.FakeFA;
			int fa_index = telnet.Controller.GetFieldAttribute(0);
			if (fa_index != -1)
				fa  = telnet.Controller.ScreenBuffer[fa_index];

			for (i = 0; i < telnet.Controller.RowCount*telnet.Controller.ColumnCount; i++) 
			{
				if (i!=0 && (i % telnet.Controller.ColumnCount)==0) 
				{
					nr++;
					ns = 0;
				}
				e = telnet.Controller.ScreenBuffer[i];
				if (FA.IsFA(e)) 
				{
					c = (byte)' ';
					fa = telnet.Controller.ScreenBuffer[i];
				}
				if (FA.IsZero(fa))
					c = (byte)' ';
				else
					c = Tables.Cg2Ascii[e];
				if (c == (byte)' ')
					ns++;
				else 
				{
					any = true;
					while (nr!=0) 
					{
						f.WriteLine();
						nr--;
					}
					while (ns!=0) 
					{
						f.WriteLine(" ");
						ns--;
					}
					f.WriteLine(System.Convert.ToChar(c));
				}
			}
			nr++;
			if (!any && !even_if_empty)
				return false;
			while (nr!=0) 
			{
				f.WriteLine();
				nr--;
			}
			return true;
		}


		/* Print the contents of the screen as text. */
		public bool PrintText_action(params object[] args)
		{
			bool secure = telnet.Appres.secure;

			if (args.Length != 1)
			{
				telnet.Events.popup_an_error("PrintText_action: requires streamwriter parameter");
				return false;
			}
			StreamWriter f = (StreamWriter)args[0];
			//	secure = True;
			fprint_screen(f, true);
			return true;
		}

		public void Dispose()
		{
			
		}
	}
}
