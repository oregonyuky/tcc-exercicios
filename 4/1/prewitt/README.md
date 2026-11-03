# Filtro de Prewitt

Na raiz do repositório, execute:

```powershell
dotnet run --project .\4\1\prewitt\Prewitt.csproj -- .\4\1\prewitt\image.png
```

O programa salva `image_prewitt.png` ao lado da imagem de entrada. Para
escolher outro caminho de saída, informe-o como segundo argumento:

```powershell
dotnet run --project .\4\1\prewitt\Prewitt.csproj -- .\caminho\imagem.png .\caminho\resultado.png
```
