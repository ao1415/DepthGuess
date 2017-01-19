using System.Threading;
using System.Threading.Tasks;

namespace DepthGuess
{
    /// <summary>
    /// 画像の深度を推測するクラス
    /// </summary>
    class Guess01
    {
        LogWriter logWriter;

        /// <summary>コンストラクタ</summary>
        /// <param name="writer"><see cref="LogWriter"/></param>
        public Guess01(LogWriter writer)
        {
            logWriter = writer;
        }

        /// <summary>画像の深度を推測する</summary>
        /// <param name="label">ラベリング結果</param>
        /// <returns>深度ラベル</returns>
        public LabelStructure GetDepth(LabelStructure label)
        {
            logWriter.Write("深さ推測を行います");

            if (label == null)
            {
                logWriter.WriteError("ラベルデータがありません");
                logWriter.WriteError("深さ推測を中止します");
                return null;
            }

            int[][] link = new RingDetection(logWriter).GetInclusionLink(label);

            int[] depthTable = new int[link.Length];

            for (int i = 0; i < link.Length; i++)
            {
                for (int j = 0; j < link[i].Length; j++)
                {
                    depthTable[link[i][j]]++;
                }
            }

            LabelStructure depthGrid = new LabelStructure(new int[label.Height, label.Width]);

            for (int y = 0; y < label.Height; y++)
            {
                for (int x = 0; x < label.Width; x++)
                {
                    int num = label[y, x];
                    depthGrid[y, x] = depthTable[num];
                }
            }

            logWriter.Write("深さ推測を行いました");
            return depthGrid;
        }

        /// <summary>画像の深度を推測する</summary>
        /// <param name="label">ラベリング結果</param>
        /// <param name="token">キャンセルトークン</param>
        /// <returns>深度ラベル</returns>
        private LabelStructure GetDepth(LabelStructure label, CancellationTokenSource token)
        {
            logWriter.Write("深さ推測を行います");

            if (label == null)
            {
                logWriter.WriteError("ラベルデータがありません");
                logWriter.WriteError("深さ推測を中止します");
                return null;
            }

            int[][] link = new RingDetection(logWriter).GetInclusionLink(label, token);
            if (token.IsCancellationRequested) return null;

            int[] depthTable = new int[link.Length];

            for (int i = 0; i < link.Length; i++)
            {
                for (int j = 0; j < link[i].Length; j++)
                {
                    depthTable[link[i][j]]++;
                }
            }

            LabelStructure depthGrid = new LabelStructure(label.Width, label.Height);

            for (int y = 0; y < label.Height; y++)
            {
                for (int x = 0; x < label.Width; x++)
                {
                    int num = label[y, x];
                    depthGrid[y, x] = depthTable[num];
                }
            }

            logWriter.Write("深さ推測を行いました");
            return depthGrid;
        }
        /// <summary>画像の深度を推測する(非同期)</summary>
        /// <param name="label">ラベリング結果</param>
        /// <param name="token">キャンセルトークン</param>
        /// <returns>深度ラベル</returns>
        public async Task<LabelStructure> GetDepthAsync(LabelStructure label, CancellationTokenSource token) { return await Task.Run(() => GetDepth(label, token)); }
    }
}
