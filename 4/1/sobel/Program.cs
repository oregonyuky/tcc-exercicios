using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

if (args.Length < 1 || args.Length > 2)
{
    Console.Error.WriteLine("Uso: Sobel <imagem-entrada> [imagem-saida.png]");
    return 1;
}

string caminhoEntrada = Path.GetFullPath(args[0]);
if (!File.Exists(caminhoEntrada))
{
    Console.Error.WriteLine("Imagem de entrada não encontrada: " + caminhoEntrada);
    return 1;
}

string caminhoSaida = args.Length == 2
    ? Path.GetFullPath(args[1])
    : Path.Combine(Path.GetDirectoryName(caminhoEntrada)!, Path.GetFileNameWithoutExtension(caminhoEntrada) + "_sobel.png");

using Bitmap original = new Bitmap(caminhoEntrada);
using Bitmap imgS = new Bitmap(original.Width, original.Height, PixelFormat.Format24bppRgb);
using (Graphics graphics = Graphics.FromImage(imgS))
{
    graphics.DrawImage(original, 0, 0, original.Width, original.Height);
}

using Bitmap imgD = new Bitmap(original.Width, original.Height, PixelFormat.Format24bppRgb);
SobelFilter.Sobel(imgS, imgD);
imgD.Save(caminhoSaida, ImageFormat.Png);
Console.WriteLine("Imagem processada salva em: " + caminhoSaida);
return 0;
