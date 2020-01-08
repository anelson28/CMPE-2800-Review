using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using System.IO;

namespace CMPE_2800Review
{

    class ByteCount
    {
        private ConcurrentQueue<byte[]> que = new ConcurrentQueue<byte[]>();
        private List<Thread> threads = new List<Thread>();

        public int TotalBlocks { get => que.Count; }

        public async void AddFiles(string[] filenames)
        {
            List<Task<byte[]>> tasklist = new List<Task<byte[]>>(filenames.Length);

            foreach (var file in filenames)
            {
                tasklist.Add(GetFileBytes(file));
            }

            while (tasklist.Count>0)
            {
                var x = await Task.WhenAny(tasklist);
                que.Enqueue(x.Result);
                tasklist.Remove(x);
            }
        }
        private async Task<byte[]> GetFileBytes(string filename)
        {

            FileInfo info = new FileInfo(filename);
            byte[] res = new byte[info.Length];
            if (info.Exists)
            {
                using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, (int)info.Length, true))
                {
                    await fs.ReadAsync(res, 0, (int)info.Length);
                }
                return res;
            }
            return null;
        }
        private int[] GetCount(byte[] sec)
        {
            int[] vs = new int[256];
            for (int i = 0; i < sec.Length; i++)
            {
                vs[sec[i]] += 1;
            }
            return vs;
        }
    }
}
