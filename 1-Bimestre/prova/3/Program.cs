using System;
using System.Drawing;
using System.IO;

public static class Program
{
    public static int Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.Error.WriteLine("Uso: ContarCelulas [caminho-da-imagem]");
            return 1;
        }

        string caminhoImagem = Path.GetFullPath(args.Length == 1
            ? args[0]
            : Path.Combine(AppContext.BaseDirectory, "cells.png"));

        if (!File.Exists(caminhoImagem))
        {
            Console.Error.WriteLine("Imagem nao encontrada: " + caminhoImagem);
            return 1;
        }

        try
        {
            using (Bitmap imagem = new Bitmap(caminhoImagem))
            {
                int quantidade = new ContarCelulas().retornarQtdCelulas(imagem);
                Console.WriteLine("Quantidade de celulas: " + quantidade);
            }

            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("Falha ao contar celulas: " + ex.Message);
            return 1;
        }
    }
}
