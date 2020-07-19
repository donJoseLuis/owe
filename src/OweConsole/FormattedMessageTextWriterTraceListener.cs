// Copyright (c) .NET Foundation and contributors. All rights reserved.

using System;
using System.Diagnostics;
using System.Threading;

namespace OweConsole
{
    internal sealed class FormattedMessageTextWriterTraceListener : TextWriterTraceListener
    {
        private volatile bool _isDisposed;


        internal FormattedMessageTextWriterTraceListener(string logPath, string name)
            :base(logPath, name)
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                if (disposing)
                {
                    base.Dispose(disposing);
                }
            }
        }


        public override void Write(string message)
        {
            _ = _isDisposed ? throw new ObjectDisposedException(string.Empty) : message;
            base.Write(FormatMessage(message));
        }

        public override void WriteLine(string message)
        {
            _ = _isDisposed ? throw new ObjectDisposedException(string.Empty) : message;
            base.WriteLine(FormatMessage(message));
        }

        private string FormatMessage(string message)
        {
            return $"{DateTime.UtcNow:dd-M-yyyy hh:mm:ss.ffffff}|{Thread.CurrentThread.ManagedThreadId}|{message}";
        }
    }
}