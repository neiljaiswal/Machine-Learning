
using System;
using System.Drawing;
using System.Windows.Forms;
using Accord.Imaging.Converters;
using Accord.MachineLearning;
using Accord.Math;
using Accord.Statistics.Distributions.DensityKernels;
using Accord.Math.Distances;
using MetroFramework.Forms;
namespace SampleApp
{
    /// <summary>
    ///   K-Means / Mean-Shift color clusterization sample application. This
    ///   application uses either K-Means or Mean-Shift to reduce the number
    ///   of colors in a sample image.
    /// </summary>
    /// 
    public partial class MainForm : Form
    {

        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        ///   Determines which radio button is selected 
        ///   and calls the appropriate algorithm.
        /// </summary>
        /// 
        private void btnRun_Click(object sender, EventArgs e)
        {
            if (radioClusters.Checked)
                runKMeans();
            else
                runMeanShift();
        }

        /// <summary>
        ///   Runs the K-Means algorithm.
        /// </summary>
        /// 
        private void runKMeans()
        {
            // Retrieve the number of clusters
            int k = (int)numClusters.Value;

            // Load original image
            Bitmap image = Properties.Resources.leaf;

            // Create converters
            ImageToArray imageToArray = new ImageToArray(min: -1, max: +1);
            ArrayToImage arrayToImage = new ArrayToImage(image.Width, image.Height, min: -1, max: +1);

            // Transform the image into an array of pixel values
            double[][] pixels; imageToArray.Convert(image, out pixels);


            // Create a K-Means algorithm using given k and a
            //  square Euclidean distance as distance metric.
            KMeans kmeans = new KMeans(k, new SquareEuclidean())
            {
                Tolerance = 0.05
            };

            // Compute the K-Means algorithm until the difference in
            //  cluster centroids between two iterations is below 0.05
            int[] idx = kmeans.Learn(pixels).Decide(pixels);


            // Replace every pixel with its corresponding centroid
            pixels.Apply((x, i) => kmeans.Clusters.Centroids[idx[i]], result: pixels);

            // Show resulting image in the picture box
            Bitmap result; arrayToImage.Convert(pixels, out result);

            pictureBox.Image = result;
        }

        /// <summary>
        ///   Runs the Mean-Shift algorithm.
        /// </summary>
        /// 
        private void runMeanShift()
        {
            int pixelSize = 3;

            // Retrieve the kernel bandwidth
            double sigma = (double)numBandwidth.Value;

            // Load original image
            Bitmap image = Properties.Resources.leaf;

            // Create converters
            ImageToArray imageToArray = new ImageToArray(min: -1, max: +1);
            ArrayToImage arrayToImage = new ArrayToImage(image.Width, image.Height, min: -1, max: +1);

            // Transform the image into an array of pixel values
            double[][] pixels; imageToArray.Convert(image, out pixels);


            // Create a MeanShift algorithm using the given bandwidth
            // and a Gaussian density kernel as the kernel function:

            IRadiallySymmetricKernel kernel = new GaussianKernel(pixelSize);

            var meanShift = new MeanShift(pixelSize, kernel, sigma)
            {
                //Tolerance = 0.05,
                //MaxIterations = 10
            };


            // Compute the mean-shift algorithm until the difference 
            // in shift vectors between two iterations is below 0.05

            int[] idx = meanShift.Learn(pixels).Decide(pixels);


            // Replace every pixel with its corresponding centroid
            pixels.Apply((x, i) => meanShift.Clusters.Modes[idx[i]], result: pixels);

            // Show resulting image in the picture box
            Bitmap result; arrayToImage.Convert(pixels, out result);

            pictureBox.Image = result;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            pictureBox.Image = Properties.Resources.leaf;
        }
    }
}
