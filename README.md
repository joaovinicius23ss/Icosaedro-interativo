<div align="center">

# 🔷 Icosaedro Interativo — Transformações Geométricas

Uma aplicação desktop para visualização e manipulação de um **icosaedro regular** projetado em 2D, com **transformações geométricas em tempo real** e **pintura interativa de faces** — construída com **C#** e **Windows Forms**.

![C#](https://img.shields.io/badge/C%23-.NET-239120?logo=csharp&logoColor=white)
![WinForms](https://img.shields.io/badge/Windows%20Forms-GDI%2B-0078D6?logo=windows&logoColor=white)
![Platform](https://img.shields.io/badge/Plataforma-Windows-lightgrey)
![License](https://img.shields.io/badge/Licen%C3%A7a-MIT-10b981)
![Status](https://img.shields.io/badge/Status-Ativo-success)

</div>

---

## 🖼️ Visão Geral

O sistema renderiza um icosaedro (sólido de 20 faces) projetado de 3D para 2D e organiza a interface em duas áreas principais, lado a lado:

| Área | Descrição |
|---|---|
| 🖌️ **Área de Desenho** | Painel onde o icosaedro é renderizado; clique em qualquer face visível para pintá-la |
| 🎛️ **Transformações Geométricas** | Sliders de translação (X/Y), rotação e escala, com valores exibidos em tempo real |
| 🎨 **Cores** | Paleta de 12 cores, pré-visualização da cor selecionada e opção de pintar toda a figura de uma vez |

> 💡 **Dica:** adicione aqui screenshots do programa em execução (`docs/preview-inicial.png`, `docs/preview-transformado.png`) para ilustrar a interface no GitHub.

---

## ✨ Funcionalidades

### 🖌️ Área de Desenho
- Projeção ortográfica do icosaedro, calculada a partir das coordenadas da razão áurea (φ).
- Apenas as **10 faces visíveis** (metade do sólido) são desenhadas e ficam clicáveis; as 3 arestas ocultas não são traçadas.
- Renderização com **antialiasing** para bordas mais suaves.

### 🎛️ Transformações Geométricas
- **Translação X** e **Translação Y** (-200 a 200 px).
- **Rotação** (-180° a 180°), em torno do centro atual da figura.
- **Escala** (10% a 300%).
- Cada slider exibe seu **valor atual** ao lado do rótulo, atualizado a cada movimento.
- Botão **"Restaurar padrão"** para zerar todas as transformações de uma vez.

### 🎨 Cores
- Paleta com **12 cores** em botões circulares.
- Painel de **pré-visualização** da cor atualmente selecionada.
- **Pintura por face**: clique em qualquer triângulo visível da figura para colori-lo com a cor selecionada.
- **Pintura geral**: um `ComboBox` permite colorir a figura inteira de uma só vez.

---

## 🧠 Conceitos de Computação Gráfica Aplicados

| Conceito | Onde está no código |
|---|---|
| Projeção 3D → 2D | `GerarVertices2D()` |
| Transformação de translação | `translacao(Point[], int, int)` |
| Transformação de rotação | `rotacao(Point[], float, Point)` |
| Transformação de escala | `escala(Point[], float, float, Point)` |
| Ocultação de faces/arestas traseiras | `VerticeOculto(int)` + array `facesVisiveis` |
| Preenchimento de polígonos | `preenchePoligono(...)` com `SolidBrush` |
| Seleção por clique (point-in-triangle) | `PontoNoTriangulo(...)` |

---

## 🧱 Tecnologias Utilizadas

- [C#](https://learn.microsoft.com/dotnet/csharp/) (.NET Framework)
- **Windows Forms** — interface gráfica
- **GDI+** (`System.Drawing` / `System.Drawing.Drawing2D`) — toda a renderização 2D

---

## 📂 Estrutura do Projeto

```
Projeto3_bim/
├── Form1.cs           # Interface, geometria e regras de transformação
├── Program.cs          # Ponto de entrada da aplicação
└── README.md            # Este arquivo
```

### Visão da arquitetura

```
┌───────────────────────────┐
│        Form1 (UI)           │
│  MontarInterface()            │
│  - Área de Desenho              │
│  - Grupo Transformações           │
│  - Grupo Cores                      │
└─────────────┬───────────────┘
              │
┌─────────────┴───────────────┐
│   Geometria do Icosaedro        │
│  GerarVertices2D()                │
│  ObterVerticesFinais()              │
│  (aplica translação → rotação →       │
│   escala sobre a projeção base)         │
└─────────────┬───────────────┘
              │
┌─────────────┴───────────────┐
│   Renderização (GDI+)           │
│  PnlDesenho_Paint()                │
│  preenchePoligono() / pintaLinha()   │
└───────────────────────────┘
```

---

## 🚀 Instalação e Execução

### Pré-requisitos
- Windows
- [Visual Studio](https://visualstudio.microsoft.com/) com a carga de trabalho **".NET desktop development"** instalada

### Passo a passo

```bash
# 1. Clone o repositório
git clone https://github.com/seu-usuario/icosaedro-interativo.git
cd icosaedro-interativo

# 2. Abra o arquivo .sln no Visual Studio

# 3. Compile e execute (F5 ou Ctrl+F5)
```

Não é necessário editar `Form1.Designer.cs` — toda a interface é montada dinamicamente em código, pelo método `MontarInterface()`.

---

## 🧪 Como Usar

1. Use os **sliders** do painel *Transformações Geométricas* para mover, girar e escalar a figura.
2. Clique em uma das **cores circulares** no painel *Cores* para selecioná-la.
3. Clique diretamente em uma **face do icosaedro** na área de desenho para pintá-la com a cor selecionada.
4. Ou use o combo **"Pintar toda a figura de uma vez"** para colorir tudo de uma só vez.
5. Clique em **"Restaurar padrão"** para voltar ao estado inicial das transformações.

---

## ⚙️ Configuração

Alguns parâmetros podem ser ajustados diretamente nas constantes no topo de `Form1.cs`:

```csharp
// Escala geral da figura na tela (pixels por unidade de projeção)
private const double ESCALA_PIXEL = 90.0;

// Ângulo de ajuste da projeção — mantém a figura alinhada
// com a ponta voltada para cima
private const double ANGULO_AJUSTE_GRAUS = 22.2388;

// Ponto central da figura na área de desenho
private const int centerX = 330;
private const int centerY = 285;
```

Os limites dos sliders (translação, rotação e escala) podem ser alterados em `CriarPainelTransformacoes()`.

---

## 🗺️ Roadmap / Próximos Passos

- [ ] Exportar a figura renderizada como imagem (PNG)
- [ ] Alternar entre diferentes sólidos platônicos (cubo, dodecaedro, etc.)
- [ ] Suporte a rotação 3D real (eixos X/Y/Z), não apenas 2D sobre a projeção
- [ ] Tema claro/escuro para a interface
- [ ] Desfazer/refazer transformações (undo/redo)

---

## 🤝 Contribuindo

Contribuições são bem-vindas! Para contribuir:

1. Faça um *fork* do projeto
2. Crie uma branch para sua funcionalidade (`git checkout -b feature/nova-funcionalidade`)
3. Faça commit das suas alterações (`git commit -m 'Adiciona nova funcionalidade'`)
4. Envie para o seu fork (`git push origin feature/nova-funcionalidade`)
5. Abra um Pull Request

---

## 📄 Licença

Este projeto está sob a licença MIT. Sinta-se livre para usar, modificar e distribuir.

---

<div align="center">

## 👤 Autor

Desenvolvido por: João Vinicius

</div>
