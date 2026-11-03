# Filtro de Sobel

Na raiz do repositório, execute:

```powershell
dotnet run --project .\4\1\sobel\Sobel.csproj -- .\4\1\sobel\image.png
```

O programa salva `image_sobel.png` ao lado da imagem de entrada. Para
escolher outro caminho de saída, informe-o como segundo argumento:

```powershell
dotnet run --project .\4\1\sobel\Sobel.csproj -- .\caminho\imagem.png .\caminho\resultado.png
```
