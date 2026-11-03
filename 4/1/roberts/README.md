# Filtro de Roberts

Na raiz do repositório, execute:

```powershell
dotnet run --project .\4\1\roberts\Roberts.csproj -- .\4\1\roberts\image.png
```

O programa salva `image_roberts.png` ao lado da imagem de entrada. Para
escolher outro caminho de saída, informe-o como segundo argumento:

```powershell
dotnet run --project .\4\1\roberts\Roberts.csproj -- .\4\1\roberts\image.png .\4\1\roberts\resultado.png
```
