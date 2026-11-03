public static int ContarVizinhos(bool[,] img, int x, int y)
{
    int cont = 0;
    for (int i = -1; i <= 1; i++) {
        for (int j = -1; j <= 1; j++) {
            if (i == 0 && j == 0) continue;
            if (img[y + i, x + j]) cont++;
        }
    }

    return cont;
}

public static List<Point> DetectarExtremidades(bool[,] img) {
    List<Point> extremidades = new List<Point>();
    for (int y = 1; y < img.GetLength(0) - 1; y++) {
        for (int x = 1; x < img.GetLength(1) - 1; x++) {
            if (img[y, x] && ContarVizinhos(img, x, y) == 1) {
                extremidades.Add(new Point(x, y));
            }
        }
    }
    return extremidades;
}

public static List<Point> ObterVizinhos(bool[,] img, int x, int y) {
    List<Point> vizinhos = new List<Point>();
    for (int i = -1; i <= 1; i++) {
        for (int j = -1; j <= 1; j++) {
            if (i == 0 && j == 0) continue;
            int nx = x + j;
            int ny = y + i;
            if (img[ny, nx])
                vizinhos.Add(new Point(nx, ny));
        }
    }
    return vizinhos;
}

public static void Pruning(bool[,] img, int tamanhoMaximo) {
    int height = img.GetLength(0);
    int width = img.GetLength(1);
    bool mudou;
    do {
        mudou = false;
        bool[,] remover = new bool[height, width];
        List<Point> extremidades = DetectarExtremidades(img);
        foreach (Point inicio in extremidades) {
            List<Point> caminho = new List<Point>();
            Point atual = inicio;
            Point anterior = new Point(-1, -1);
            caminho.Add(atual);
            while (true) {
                List<Point> vizinhos = ObterVizinhos( img, atual.X, atual.Y);
                // Encontrou uma bifurcação
                if (vizinhos.Count >= 3) {
                    if (caminho.Count <= tamanhoMaximo) {
                        foreach (Point p in caminho)
                            remover[p.Y, p.X] = true;
                    }
                    break;
                }

                // Chegou a outra extremidade ou a um pixel isolado
                if (vizinhos.Count <= 1 && caminho.Count > 1) break;
                Point proximo = new Point(-1, -1);

                foreach (Point p in vizinhos) {
                    if (p != anterior) {
                        proximo = p;
                        break;
                    }
                }
                
                if (proximo.X == -1)
                    break;
                anterior = atual;
                atual = proximo;
                caminho.Add(atual);
            }
        }

        // Remover os pixels somente depois de analisar os caminhos
        for (int y = 1; y < height - 1; y++) {
            for (int x = 1; x < width - 1; x++) {
                if (remover[y, x]) {
                    img[y, x] = false;
                    mudou = true;
                }
            }
        }

    } while (mudou);
}