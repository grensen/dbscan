
## DBSCAN

DBSCAN (Density-Based Spatial Clustering of Applications with Noise) is a clustering algorithm used in machine learning. It groups data points based on their density rather than predefined clusters, making it effective for irregular datasets. However, it may face limitations with datasets containing more than 1000 samples.

## DBSCAN Simple Example Demo

<p align="center">
    <img src="https://github.com/grensen/dbscan/blob/main/dbscan_jmc_demo.png" >
</p>

The demo, written by James McCaffrey in [this blog post about DBSCAN](https://jamesmccaffrey.wordpress.com/2023/10/23/data-dbscan-clustering-from-scratch-using-csharp/), is originally in C#. [This](https://jamesmccaffrey.wordpress.com/2023/10/19/data-dbscan-clustering-from-scratch-using-python/) particular demo is in Python, leveraging a library. Importantly, the Python reference demo and the C# DBSCAN demo produce identical results. 

To install the demo, simply create a console application in Visual Studio 2022 and copy the [code](https://github.com/grensen/dbscan/blob/main/dbscan_jmc_demo.cs) to run.

## DBSCAN MNIST Demo

<p align="center">
    <img src="https://github.com/grensen/dbscan/blob/main/dbscan_mnist.png" >
</p>

This is a visualization of the best cluster I've found for the first 1000 MNIST training examples. It may be more interesting than practical. However, the quality of the clusters, in some cases, is quite impressive.

To install the demo, create a WPF application in Visual Studio 2022, follow [this](https://raw.githubusercontent.com/grensen/custom_connect/main/figures/install.gif?raw=true) animation, and copy the [code](https://github.com/grensen/dbscan/blob/main/dbscan_wpf_visual_demo.cs) to run.

If you've found this repository interesting, make sure to take a look at [the companion repository for K-Means++ clustering](https://github.com/grensen/k-means).

