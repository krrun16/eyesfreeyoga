using System;
using System.Threading;

public class FileWriter
{
    String logPath;
    public FileWriter(String logPath)
    {
        this.logPath = logPath;
    }
    
    private ReaderWriterLockSlim lock_ = new ReaderWriterLockSlim();
    public void WriteData(string data)
    {
        lock_.EnterWriteLock();
        try
        {
            System.IO.File.AppendAllText(logPath, data + "\n");
        }
        finally
        {
            lock_.ExitWriteLock();
        }
    }

}