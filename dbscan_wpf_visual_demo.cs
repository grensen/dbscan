// https://jamesmccaffrey.wordpress.com/2023/10/23/data-dbscan-clustering-from-scratch-using-csharp/

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;

public class TheWindow : Window
{
    // DBSCAN hyperparameters:
    int maxSamples = 1000;
    double epsilon = 6.0;
    int minPoints = 6;

    readonly string path = @"C:\DBSCAN\"; // autodata mnist from my github to folder
    readonly SolidColorBrush brFont = new(Color.FromRgb(205, 199, 168));
    readonly SolidColorBrush brFont2 = new(Color.FromRgb(9, 6, 0));
    readonly Canvas canGlobal = new();

    int mnistX = 20;
    int pseudoK = 8; // visual space possible clusters, 8 for demo
    // support 
    System.Globalization.CultureInfo ci = System.Globalization.CultureInfo.GetCultureInfo("en-us");
    readonly Typeface tf = new("TimesNewRoman"); // "Arial"

    [STAThread]
    public static void Main() { new Application().Run(new TheWindow()); }

    // CONSTRUCTOR - LOADED - ONINIT
    private TheWindow() // constructor
    {
        Title = "McCaffrey DBSCAN On MNIST"; //        
        Content = canGlobal;
        Background = Brushes.Black; 
        Width = pseudoK * 160 + 50;
        Height = 830; // HeightG;                   

        DrawingContext dc = ContextHelpMod(false, ref canGlobal);
        DBSCAN_Visual_MNIST(ref dc);
        dc.Close();
        Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action(delegate { }));
    } // TheWindow end

    void DBSCAN_Visual_MNIST(ref DrawingContext dc)
    {

        AutoData d = new AutoData(path);
        // convert data to 2D array
        double[][] data = new double[maxSamples][];
        for (int i = 0, c = 0; i < maxSamples; i++)
        {
            data[i] = new double[784]; // mnist sample size
            for (int j = 0; j < 784; j++)
            {
                var s = d.samplesTrainingF[c++];
                if(s > 0)
                data[i][j] = s;
               // else  data[i][j] = -0.01;
            }
        }

        MyDBSCAN dbscan = new MyDBSCAN(epsilon, minPoints);
        int[] clustering = dbscan.Cluster(data);

        int clusters = 0;
        for (int i = 0; i < clustering.Length; i++)
            if (clustering[i] > clusters) clusters = clustering[i];
        clusters++;

        int[] clusterCount = new int[clusters];
        for (int i = 0; i < clustering.Length; i++)
            if (clustering[i] >= 0) clusterCount[clustering[i]]++;
        
        int noise = 0;
        for (int i = 0; i < clustering.Length; i++)
            if (clustering[i] == -1) noise++;

        dc.DrawText(new FormattedText("McCaffrey DBSCAN Clustering on MNIST", ci, FlowDirection.LeftToRight, tf, 12, brFont, VisualTreeHelper.GetDpi(this).PixelsPerDip),
            new Point(mnistX + 1, 20));
        dc.DrawText(new FormattedText("Samples = " + maxSamples.ToString(), ci, FlowDirection.LeftToRight, tf, 12, brFont, VisualTreeHelper.GetDpi(this).PixelsPerDip),
            new Point(mnistX + 1, 35));
        dc.DrawText(new FormattedText("Estimated K = " + clusters.ToString(), ci, FlowDirection.LeftToRight, tf, 12, brFont, VisualTreeHelper.GetDpi(this).PixelsPerDip),
            new Point(mnistX + 1, 50));

        dc.DrawText(new FormattedText("Epsilon = " + epsilon.ToString("F2"), ci, FlowDirection.LeftToRight, tf, 12, brFont, VisualTreeHelper.GetDpi(this).PixelsPerDip),
            new Point(mnistX + 1, 65));
        dc.DrawText(new FormattedText("MinPoints = " + minPoints.ToString(), ci, FlowDirection.LeftToRight, tf, 12, brFont, VisualTreeHelper.GetDpi(this).PixelsPerDip),
            new Point(mnistX + 1, 80));

        dc.DrawText(new FormattedText("Cluster count:", ci, FlowDirection.LeftToRight, tf, 12, brFont, VisualTreeHelper.GetDpi(this).PixelsPerDip),
            new Point(mnistX + 1, 110));
        dc.DrawText(new FormattedText("Cluster samples:", ci, FlowDirection.LeftToRight, tf, 12, brFont, VisualTreeHelper.GetDpi(this).PixelsPerDip),
            new Point(mnistX + 1, 162));
        
        for (int i = 0; i < clusters; i++)
        {
            dc.DrawRectangle(brFont, null, new Rect(mnistX + i * 160, 131, 80, 20));
            dc.DrawText(new FormattedText(i.ToString() + " = " + clusterCount[i].ToString(), ci, FlowDirection.LeftToRight, tf, 10, brFont2, VisualTreeHelper.GetDpi(this).PixelsPerDip),
                new Point(mnistX + i * 160 + 5, 135));
        }

        dc.DrawRectangle(brFont, null, new Rect(mnistX + clusters * 160, 131, 80, 20));
        dc.DrawText(new FormattedText("Noise = " + noise.ToString(), ci, FlowDirection.LeftToRight, tf, 12, brFont2, VisualTreeHelper.GetDpi(this).PixelsPerDip),
            new Point(mnistX + clusters * 160 + 5, 135));

        int[] classCount = new int[clusters];
        int noiseCount = 0;
        for (int pid = 0; pid < maxSamples; pid++)
        {
            int nid = clustering[pid];
            if (nid < 0)   
            {
                if (noiseCount < 100) 
                    for (int i = 0, c = 0; i < 28; i++)
                        for (int j = 0; j < 28; j++, c++)
                        {
                            float nj = (float)data[pid][c];
                            if (nj > 0.35) // cut the lows for peformence
                                dc.DrawRectangle(BrF(255 * nj, 180 * nj, 0),
                                    null, new Rect(mnistX + 1 * j + (clusters * 160 + ((noiseCount) % 5) * 28), 1 * i + 100 + 80 + ((noiseCount) / 5) * 28, 1, 1));
                        }
                noiseCount++;
                continue;
            }

            int nid2 = classCount[nid];
            if (nid2 < 100)          
                for (int i = 0, c = 0; i < 28; i++)
                    for (int j = 0; j < 28; j++, c++)
                    {
                        float nj = (float)data[pid][c];
                        if (nj > 0.35) // cut the lows for peformence
                            dc.DrawRectangle(BrF(255 * nj, 180 * nj, 0),
                                null, new Rect(mnistX + 1 * j + (nid * 160 + (nid2 % 5) * 28), 1 * i + 100 + 80 + (nid2 / 5) * 28, 1, 1));
                    }
            classCount[nid]++;
        }
    }
    // DATA
    struct AutoData
    {
        public byte[] labelsTraining, labelsTest;
        public float[] samplesTrainingF, samplesTestF;

        static float[] NormalizeData(byte[] samples)
        {
            float[] samplesF = new float[samples.Length];
            for (int i = 0; i < samples.Length; i++)
                samplesF[i] = samples[i] / 255f;
            return samplesF;
        }

        public AutoData(string yourPath)
        {
            // Hardcoded URLs from my GitHub
            string trainDataUrl = "https://github.com/grensen/gif_test/raw/master/MNIST_Data/train-images.idx3-ubyte";
            string trainLabelUrl = "https://github.com/grensen/gif_test/raw/master/MNIST_Data/train-labels.idx1-ubyte";
            string testDataUrl = "https://github.com/grensen/gif_test/raw/master/MNIST_Data/t10k-images.idx3-ubyte";
            string testLabelUrl = "https://github.com/grensen/gif_test/raw/master/MNIST_Data/t10k-labels.idx1-ubyte";

            byte[] test, training;

            // Change variable names for readability
            string trainDataPath = "trainData", trainLabelPath = "trainLabel", testDataPath = "testData", testLabelPath = "testLabel";

            if (!File.Exists(Path.Combine(yourPath, trainDataPath))
                || !File.Exists(Path.Combine(yourPath, trainLabelPath))
                || !File.Exists(Path.Combine(yourPath, testDataPath))
                || !File.Exists(Path.Combine(yourPath, testLabelPath)))
            {
                Console.WriteLine("Status: MNIST Dataset Not found");
                if (!Directory.Exists(yourPath)) Directory.CreateDirectory(yourPath);

                // Padding bits: data = 16, labels = 8
                Console.WriteLine("Action: Downloading and Cleaning the Dataset from GitHub");
                training = new HttpClient().GetAsync(trainDataUrl).Result.Content.ReadAsByteArrayAsync().Result.Skip(16).Take(60000 * 784).ToArray();
                labelsTraining = new HttpClient().GetAsync(trainLabelUrl).Result.Content.ReadAsByteArrayAsync().Result.Skip(8).Take(60000).ToArray();
                test = new HttpClient().GetAsync(testDataUrl).Result.Content.ReadAsByteArrayAsync().Result.Skip(16).Take(10000 * 784).ToArray();
                labelsTest = new HttpClient().GetAsync(testLabelUrl).Result.Content.ReadAsByteArrayAsync().Result.Skip(8).Take(10000).ToArray();

                Console.WriteLine("Save Path: " + yourPath + "\n");
                File.WriteAllBytesAsync(Path.Combine(yourPath, trainDataPath), training);
                File.WriteAllBytesAsync(Path.Combine(yourPath, trainLabelPath), labelsTraining);
                File.WriteAllBytesAsync(Path.Combine(yourPath, testDataPath), test);
                File.WriteAllBytesAsync(Path.Combine(yourPath, testLabelPath), labelsTest);
            }
            else
            {
                // Data exists on the system, just load from yourPath
                Console.WriteLine("Dataset: MNIST (" + yourPath + ")" + "\n");
                training = File.ReadAllBytes(Path.Combine(yourPath, trainDataPath)).Take(60000 * 784).ToArray();
                labelsTraining = File.ReadAllBytes(Path.Combine(yourPath, trainLabelPath)).Take(60000).ToArray();
                test = File.ReadAllBytes(Path.Combine(yourPath, testDataPath)).Take(10000 * 784).ToArray();
                labelsTest = File.ReadAllBytes(Path.Combine(yourPath, testLabelPath)).Take(10000).ToArray();
            }

            samplesTrainingF = NormalizeData(training);
            samplesTestF = NormalizeData(test);
        }
    }

    public class MyDBSCAN
    {
        public double eps;
        public int minPts;
        public double[][] data;  // supplied in cluster()
        public int[] labels;  // supplied in cluster()

        public MyDBSCAN(double eps, int minPts)
        {
            this.eps = eps;
            this.minPts = minPts;
        }

        public int[] Cluster(double[][] data)
        {
            this.data = data;  // by reference
            this.labels = new int[this.data.Length];
            for (int i = 0; i < labels.Length; ++i)
                this.labels[i] = -2;  // unprocessed

            int cid = -1;  // offset the start
            for (int i = 0; i < this.data.Length; ++i)
            {
                if (this.labels[i] != -2)  // has been processed
                    continue;

                List<int> neighbors = this.RegionQuery(i);
                if (neighbors.Count < this.minPts)
                {
                    this.labels[i] = -1;  // noise
                }
                else
                {
                    ++cid;
                    this.Expand(i, neighbors, cid);
                }
            }

            return this.labels;
        }

        private List<int> RegionQuery(int p)
        {
            List<int> result = new List<int>();
            for (int i = 0; i < this.data.Length; ++i)
            {
                double dist = EucDistance(this.data[p], this.data[i]);
                if (dist < this.eps)
                    result.Add(i);
            }
            return result;
        }

        private void Expand(int p, List<int> neighbors, int cid)
        {
            this.labels[p] = cid;
            for (int i = 0; i < neighbors.Count; ++i)
            {
                int pn = neighbors[i];
                if (this.labels[pn] == -1)  // noise
                    this.labels[pn] = cid;
                else if (this.labels[pn] == -2)  // unprocessed
                {
                    this.labels[pn] = cid;
                    List<int> newNeighbors = this.RegionQuery(pn);
                    if (newNeighbors.Count >= this.minPts)
                        neighbors.AddRange(newNeighbors);
                }
            } // for
        }

        private static double EucDistance(double[] x1,
          double[] x2)
        {
            int dim = x1.Length;
            double sum = 0.0;
            for (int i = 0; i < dim; ++i)
                sum += (x1[i] - x2[i]) * (x1[i] - x2[i]);
            return Math.Sqrt(sum);
        }

    } // class DBSCAN

    static DrawingContext ContextHelpMod(bool isInit, ref Canvas cTmp)
    {
        if (!isInit) cTmp.Children.Clear();
        DrawingVisualElement drawingVisual = new();
        cTmp.Children.Add(drawingVisual);
        return drawingVisual.drawingVisual.RenderOpen();
    }
    static Brush BrF(float red, float green, float blue)
    {
        Brush frozenBrush = new SolidColorBrush(Color.FromRgb((byte)red, (byte)green, (byte)blue));
        frozenBrush.Freeze();
        return frozenBrush;
    }
} // TheWindow end

public class DrawingVisualElement : FrameworkElement
{
    private readonly VisualCollection _children;
    public DrawingVisual drawingVisual;
    public DrawingVisualElement()
    {
        _children = new VisualCollection(this);
        drawingVisual = new DrawingVisual();
        _children.Add(drawingVisual);
    }
    protected override int VisualChildrenCount
    {
        get { return _children.Count; }
    }
    protected override Visual GetVisualChild(int index)
    {
        if (index < 0 || index >= _children.Count)
            throw new();
        return _children[index];
    }
} // DrawingVisualElement



