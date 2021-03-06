// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System 
{
	[StructLayout (LayoutKind.Auto)]
	public ref struct ArgIterator
	{
#pragma warning disable 169, 414
		IntPtr sig;
		IntPtr args;
		int next_arg;
		int num_args;
#pragma warning restore 169, 414

		[MethodImpl (MethodImplOptions.InternalCall)]
		extern void Setup (IntPtr argsp, IntPtr start);

		public ArgIterator (RuntimeArgumentHandle arglist)
		{
			sig = IntPtr.Zero;
			args = IntPtr.Zero;
			next_arg = num_args = 0;
			if (arglist.args == IntPtr.Zero)
				throw new PlatformNotSupportedException ();
			Setup (arglist.args, IntPtr.Zero);
		}

		[CLSCompliant (false)]
		unsafe public ArgIterator (RuntimeArgumentHandle arglist, void *ptr)
		{
			sig = IntPtr.Zero;
			args = IntPtr.Zero;
			next_arg = num_args = 0;
			if (arglist.args == IntPtr.Zero)
				throw new PlatformNotSupportedException ();
			Setup (arglist.args, (IntPtr) ptr);
		}

		public void End ()
		{
			next_arg = num_args;
		}

		public override bool Equals (object? o)
		{
			throw new NotSupportedException ("ArgIterator does not support Equals.");
		}

		public override int GetHashCode ()
		{
			return sig.GetHashCode ();
		}

		[CLSCompliant (false)]
		public TypedReference GetNextArg ()
		{
			if (num_args == next_arg)
				throw new InvalidOperationException ("Invalid iterator position.");
			TypedReference result = new TypedReference ();
			unsafe {
				IntGetNextArg (&result);
			}
			return result;
		}

		[MethodImpl (MethodImplOptions.InternalCall)]
		extern unsafe void IntGetNextArg (void *res);

		[CLSCompliant (false)]
		public TypedReference GetNextArg (RuntimeTypeHandle rth)
		{
			if (num_args == next_arg)
				throw new InvalidOperationException ("Invalid iterator position.");
			TypedReference result = new TypedReference ();
			unsafe {
				IntGetNextArgWithType (&result, rth.Value);
			}
			return result;
		}

		[MethodImpl (MethodImplOptions.InternalCall)]
		extern unsafe void IntGetNextArgWithType (void *res, IntPtr rth);

		public RuntimeTypeHandle GetNextArgType ()
		{
			if (num_args == next_arg)
				throw new InvalidOperationException ("Invalid iterator position.");
			return new RuntimeTypeHandle (IntGetNextArgType ());
		}

		[MethodImpl (MethodImplOptions.InternalCall)]
		extern IntPtr IntGetNextArgType ();

		public int GetRemainingCount ()
		{
			return num_args - next_arg;
		}
	}
}
