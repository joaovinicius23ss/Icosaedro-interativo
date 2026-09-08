using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Projeto3_bim
{
    public partial class Form1 : Form
    {
        // primitivas basicas
        private Color cor(int r, int g, int b)
        {
            return Color.FromArgb(r, g, b);
        }

        private Pen caneta(int r, int g, int b, float espessura)
        {
            return new Pen(cor(r, g, b), espessura);
        }

        private SolidBrush fundo(int r, int g, int b)
        {
            return new SolidBrush(cor(r, g, b));
        }
        private void pintaLinha(PaintEventArgs e, Pen caneta, int x1, int y1, int x2, int y2)
        {
            e.Graphics.DrawLine(caneta, x1, y1, x2, y2);
        }

        private void preenchePoligono(PaintEventArgs e, SolidBrush fundo, Point[] pontos)
        {
            e.Graphics.FillPolygon(fundo, pontos);
        }

        private Point[] translacao(Point[] pontos, int deslocX, int deslocY)
        {
            Point[] resultado = new Point[pontos.Length];

            for (int i = 0; i < pontos.Length; i++)
            {
                resultado[i].X = pontos[i].X + deslocX;
                resultado[i].Y = pontos[i].Y + deslocY;
            }

            return resultado;
        }

        private Point[] escala(Point[] pontos, float escalaX, float escalaY, Point centro)
        {
            Point[] resultado = new Point[pontos.Length];

            for (int i = 0; i < pontos.Length; i++)
            {
                resultado[i].X = (int)(centro.X + (pontos[i].X - centro.X) * escalaX);
                resultado[i].Y = (int)(centro.Y + (pontos[i].Y - centro.Y) * escalaY);
            }

            return resultado;
        }

        private Point[] rotacao(Point[] pontos, float anguloGraus, Point centro)
        {
            Point[] resultado = new Point[pontos.Length];

            double rad = anguloGraus * Math.PI / 180.0;
            double cosA = Math.Cos(rad);
            double sinA = Math.Sin(rad);

            for (int i = 0; i < pontos.Length; i++)
            {
                int dx = pontos[i].X - centro.X;
                int dy = pontos[i].Y - centro.Y;

                resultado[i].X = (int)(centro.X + dx * cosA - dy * sinA);
                resultado[i].Y = (int)(centro.Y + dx * sinA + dy * cosA);
            }

            return resultado;
        }

        // dados do icosaedro regular, definidos a partir da razão áurea PHI = (1 + sqrt(5)) / 2
        private const double PHI = 1.6180339887;


        // Os 12 vértices do icosaedro regular, definidos a partir da razão áurea.
        private static readonly double[] coordX = { -1, 1, -1, 1, 0, 0, 0, 0, PHI, PHI, -PHI, -PHI };
        private static readonly double[] coordY = { PHI, PHI, -PHI, -PHI, -1, 1, -1, 1, 0, 0, 0, 0 };
        private static readonly double[] coordZ = { 0, 0, 0, 0, PHI, PHI, -PHI, -PHI, -1, 1, -1, 1 };

        // Eixos de projeção 
        private const double EIXO_X_X = -0.70710678, EIXO_X_Y = 0.0, EIXO_X_Z = 0.70710678;
        private const double EIXO_Y_X = -0.40824829, EIXO_Y_Y = 0.81649658, EIXO_Y_Z = -0.40824829;
        private const double ANGULO_AJUSTE_GRAUS = 22.2388;
        private const double ESCALA_PIXEL = 90.0;
        private const int centerX = 330;
        private const int centerY = 285;

        private readonly int[,] arestas = new int[,]
        {
            {0,11},{11,5},{0,5},{1,5},{0,1},{1,7},{0,7},{7,10},{0,10},{10,11},
            {5,9},{1,9},{11,4},{4,5},{10,2},{2,11},{7,6},{6,10},{1,8},{7,8},
            {3,9},{4,9},{3,4},{4,2},{2,3},{2,6},{3,6},{6,8},{3,8},{8,9}
        };

        private readonly int[,] faces = new int[,]
        {
            {0,11,5}, {0,5,1}, {0,1,7}, {0,7,10}, {0,10,11},
            {1,5,9},  {5,11,4},{11,10,2},{10,7,6}, {7,1,8},
            {3,9,4},  {3,4,2}, {3,2,6}, {3,6,8},  {3,8,9},
            {4,9,5},  {2,4,11},{6,2,10},{8,6,7},  {9,8,1}
        };

        private readonly int[] facesVisiveis = { 0, 1, 2, 5, 6, 9, 10, 14, 15, 19 };

        private SolidBrush[] fundosFaces = new SolidBrush[10];

        private bool VerticeOculto(int indice)
        {
            return indice == 2 || indice == 6 || indice == 10;
        }

    


        // estado de aplicação 
        private int offsetX = 0;
        private int offsetY = 0;
        private float escalaAtual = 1.0f;
        private float anguloRotacao = 0f;

        private int rAtual = 0;
        private int gAtual = 0;
        private int bAtual = 0;





        // componentes de interface 
        private Panel pnlDesenho;
        private GroupBox grpTransformacoes;
        private TrackBar tbTranslacaoX;
        private TrackBar tbTranslacaoY;
        private TrackBar tbRotacao;
        private TrackBar tbEscala;
        private Label lblValorTX;
        private Label lblValorTY;
        private Label lblValorRot;
        private Label lblValorEsc;
        private Button btnResetar;
        private GroupBox grpCores;
        private Panel pnlCorSelecionada;
        private ComboBox cmbCorGeral;

        public Form1()
        {
            MontarInterface();
            for (int i = 0; i < fundosFaces.Length; i++)
                fundosFaces[i] = fundo(255, 255, 255);
        }


        // mostrar interface
        private void MontarInterface()
        {
            Width = 1010;
            Height = 760;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(74, 222, 128);
            Font = new Font("Segoe UI", 9f);

            Label lblTitulo = new Label();
            lblTitulo.Text = "ICOSAEDRO - PROJETO BIMESTRAL";
            lblTitulo.Font = new Font("Segoe UI", 13f, FontStyle.Bold);
            lblTitulo.ForeColor = Color.FromArgb(45, 55, 75);
            lblTitulo.AutoSize = true;
            lblTitulo.Location = new Point(20, 15);
            Controls.Add(lblTitulo);

            CriarAreaDesenho();
            CriarPainelTransformacoes();
            CriarPainelCores();
        }

        private void CriarAreaDesenho()
        {
            pnlDesenho = new Panel();
            pnlDesenho.Location = new Point(20, 55);
            pnlDesenho.Size = new Size(630, 650);
            pnlDesenho.BackColor = Color.White;
            pnlDesenho.BorderStyle = BorderStyle.FixedSingle;
            pnlDesenho.Paint += PnlDesenho_Paint;
            pnlDesenho.MouseClick += PnlDesenho_MouseClick;
            Controls.Add(pnlDesenho);

            Label lblDica = new Label();
            lblDica.ForeColor = Color.Gray;
            lblDica.AutoSize = true;
            lblDica.Location = new Point(22, 710);
            Controls.Add(lblDica);
        }

        private void CriarPainelTransformacoes()
        {
            grpTransformacoes = new GroupBox();
            grpTransformacoes.Text = "Transformações Geométricas";
            grpTransformacoes.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            grpTransformacoes.Location = new Point(670, 55);
            grpTransformacoes.Size = new Size(310, 290);
            Controls.Add(grpTransformacoes);

            int y = 30;

            CriarSliderComRotulo(grpTransformacoes, "Translação X", ref tbTranslacaoX, ref lblValorTX,
                -200, 200, 0, ref y);
            CriarSliderComRotulo(grpTransformacoes, "Translação Y", ref tbTranslacaoY, ref lblValorTY,
                -200, 200, 0, ref y);
            CriarSliderComRotulo(grpTransformacoes, "Rotação (graus)", ref tbRotacao, ref lblValorRot,
                -180, 180, 0, ref y);
            CriarSliderComRotulo(grpTransformacoes, "Escala (%)", ref tbEscala, ref lblValorEsc,
                10, 300, 100, ref y);

            btnResetar = new Button();
            btnResetar.Text = "Restaurar padrão";
            btnResetar.Location = new Point(20, y + 5);
            btnResetar.Size = new Size(270, 32);
            btnResetar.FlatStyle = FlatStyle.Flat;
            btnResetar.BackColor = Color.FromArgb(225, 229, 237);
            btnResetar.Click += BtnResetar_Click;
            grpTransformacoes.Controls.Add(btnResetar);
        }

        private void CriarSliderComRotulo(GroupBox grupo, string titulo, ref TrackBar slider, ref Label lblValor,
            int minimo, int maximo, int valorInicial, ref int y)
        {
            Label lblTitulo = new Label();
            lblTitulo.Text = titulo;
            lblTitulo.AutoSize = true;
            lblTitulo.Location = new Point(20, y);
            grupo.Controls.Add(lblTitulo);

            Label valor = new Label();
            valor.Text = valorInicial.ToString();
            valor.AutoSize = true;
            valor.ForeColor = Color.FromArgb(80, 100, 160);
            valor.Location = new Point(250, y);
            grupo.Controls.Add(valor);
            lblValor = valor;

            TrackBar tb = new TrackBar();
            tb.Minimum = minimo;
            tb.Maximum = maximo;
            tb.Value = valorInicial;
            tb.TickFrequency = Math.Max(1, (maximo - minimo) / 10);
            tb.Location = new Point(15, y + 18);
            tb.Width = 275;
            tb.Scroll += TbTransformacao_Scroll;
            grupo.Controls.Add(tb);
            slider = tb;

            y += 65;
        }

        private void CriarPainelCores()
        {
            grpCores = new GroupBox();
            grpCores.Text = "Cores";
            grpCores.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            grpCores.Location = new Point(670, 370);
            grpCores.Size = new Size(310, 335);
            Controls.Add(grpCores);

            string[] nomesCores = {
                "Vermelho", "Laranja", "Amarelo", "Verde",
                "Azul", "Roxo", "Marrom", "Cinza",
                "Preto", "Branco", "Rosa", "Ciano" 
            };

            Color[] paletaCores = {
                Color.Red, Color.Orange, Color.Yellow, Color.Green,
                Color.Blue, Color.Purple, Color.Sienna, Color.Silver,
                Color.Black, Color.White, Color.Pink, Color.Cyan
            };

            Label lblPaleta = new Label();
            lblPaleta.Text = "Clique em uma cor para selecioná-la:";
            lblPaleta.AutoSize = true;
            lblPaleta.Location = new Point(20, 28);
            grpCores.Controls.Add(lblPaleta);

            int colunas = 6;
            int tamanhoBotao = 34;
            int espaco = 6;
            for (int i = 0; i < nomesCores.Length; i++)
            {
                int linha = i / colunas;
                int coluna = i % colunas;

                Button btn = new Button();
                btn.Name = nomesCores[i];
                btn.BackColor = paletaCores[i];
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderColor = Color.FromArgb(160, 160, 160);
                btn.FlatAppearance.BorderSize = 1;
                btn.Cursor = Cursors.Hand;
                btn.Location = new Point(20 + coluna * (tamanhoBotao + espaco), 50 + linha * (tamanhoBotao + espaco));
                btn.Size = new Size(tamanhoBotao, tamanhoBotao);
                btn.Region = new Region(CriarCirculo(tamanhoBotao, tamanhoBotao));
                btn.Click += BtnCor_Click;
                grpCores.Controls.Add(btn);
            }

            Label lblSelecionada = new Label();
            lblSelecionada.Text = "Cor selecionada:";
            lblSelecionada.AutoSize = true;
            lblSelecionada.Location = new Point(20, 130);
            grpCores.Controls.Add(lblSelecionada);

            pnlCorSelecionada = new Panel();
            pnlCorSelecionada.BackColor = Color.Black;
            pnlCorSelecionada.BorderStyle = BorderStyle.FixedSingle;
            pnlCorSelecionada.Location = new Point(150, 128);
            pnlCorSelecionada.Size = new Size(50, 24);
            grpCores.Controls.Add(pnlCorSelecionada);

            Label lblCorGeral = new Label();
            lblCorGeral.Text = "Pintar toda a figura de uma vez:";
            lblCorGeral.AutoSize = true;
            lblCorGeral.Location = new Point(20, 175);
            grpCores.Controls.Add(lblCorGeral);

            cmbCorGeral = new ComboBox();
            cmbCorGeral.Location = new Point(20, 198);
            cmbCorGeral.Width = 260;
            cmbCorGeral.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCorGeral.Items.AddRange(new object[]
            {
                "Vermelho", "Azul", "Verde", "Amarelo", "Roxo",
                "Laranja", "Preto", "Branco", "Cinza", "Ciano"
            });
            cmbCorGeral.SelectedIndexChanged += CmbCorGeral_SelectedIndexChanged;
            grpCores.Controls.Add(cmbCorGeral);
        }

        private GraphicsPath CriarCirculo(int largura, int altura)
        {
            GraphicsPath caminho = new GraphicsPath();
            caminho.AddEllipse(0, 0, largura, altura);
            return caminho;
        }


        // projeções 2D
        private Point[] GerarVertices2D()
        {
            Point[] pontos = new Point[12];
            double anguloAjusteRad = ANGULO_AJUSTE_GRAUS * Math.PI / 180.0;
            double cosAjuste = Math.Cos(anguloAjusteRad);
            double sinAjuste = Math.Sin(anguloAjusteRad);

            for (int i = 0; i < 12; i++)
            {
                double sx = coordX[i] * EIXO_X_X + coordY[i] * EIXO_X_Y + coordZ[i] * EIXO_X_Z;
                double sy = coordX[i] * EIXO_Y_X + coordY[i] * EIXO_Y_Y + coordZ[i] * EIXO_Y_Z;

                // Gira a projeção para que a figura fique com a mesma inclinação
                // do modelo de referência (vértice voltado para cima).
                double sxFinal = sx * cosAjuste - sy * sinAjuste;
                double syFinal = sx * sinAjuste + sy * cosAjuste;

                int px = centerX + (int)(sxFinal * ESCALA_PIXEL);
                int py = centerY - (int)(syFinal * ESCALA_PIXEL);

                pontos[i] = new Point(px, py);
            }

            return pontos;
        }
        private Point[] ObterVerticesFinais()
        {
            Point[] verticesBase = GerarVertices2D();
            Point[] copia = (Point[])verticesBase.Clone();
            Point[] transladados = translacao(copia, offsetX, offsetY);
            Point centroFigura = new Point(centerX + offsetX, centerY + offsetY);
            Point[] rotacionados = rotacao(transladados, anguloRotacao, centroFigura);
            return escala(rotacionados, escalaAtual, escalaAtual, centroFigura);
        }
        private bool PontoNoTriangulo(Point p, Point p1, Point p2, Point p3)
        {
            float d1 = (p.X - p2.X) * (p1.Y - p2.Y) - (p1.X - p2.X) * (p.Y - p2.Y);
            float d2 = (p.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p.Y - p3.Y);
            float d3 = (p.X - p1.X) * (p3.Y - p1.Y) - (p3.X - p1.X) * (p.Y - p1.Y);

            bool temNegativo = (d1 < 0) || (d2 < 0) || (d3 < 0);
            bool temPositivo = (d1 > 0) || (d2 > 0) || (d3 > 0);

            return !(temNegativo && temPositivo);
        }



        // eventos
        private void PnlDesenho_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Point[] verticesFinais = ObterVerticesFinais();

            for (int k = 0; k < facesVisiveis.Length; k++)
            {
                int indiceFace = facesVisiveis[k];

                Point[] pontosFace = new Point[3];
                for (int j = 0; j < 3; j++)
                    pontosFace[j] = verticesFinais[faces[indiceFace, j]];

                preenchePoligono(e, fundosFaces[k], pontosFace);
            }

            Pen canetaAresta = caneta(60, 60, 60, 2f);
            for (int i = 0; i < arestas.GetLength(0); i++)
            {
                int a = arestas[i, 0];
                int b = arestas[i, 1];

                if (!VerticeOculto(a) && !VerticeOculto(b))
                {
                    pintaLinha(e, canetaAresta,
                        verticesFinais[a].X, verticesFinais[a].Y,
                        verticesFinais[b].X, verticesFinais[b].Y);
                }
            }
        }

        private void PnlDesenho_MouseClick(object sender, MouseEventArgs e)
        {
            Point[] verticesFinais = ObterVerticesFinais();

            for (int k = 0; k < facesVisiveis.Length; k++)
            {
                int indiceFace = facesVisiveis[k];

                Point[] pontosFace = new Point[3];
                for (int j = 0; j < 3; j++)
                    pontosFace[j] = verticesFinais[faces[indiceFace, j]];

                if (PontoNoTriangulo(e.Location, pontosFace[0], pontosFace[1], pontosFace[2]))
                {
                    fundosFaces[k] = fundo(rAtual, gAtual, bAtual);
                    pnlDesenho.Invalidate();
                    return;
                }
            }
        }

        private void BtnCor_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            switch (btn.Name)
            {
                case "Vermelho": rAtual = 255; gAtual = 0; bAtual = 0; break;
                case "Laranja": rAtual = 255; gAtual = 165; bAtual = 0; break;
                case "Amarelo": rAtual = 255; gAtual = 255; bAtual = 0; break;
                case "Verde": rAtual = 0; gAtual = 128; bAtual = 0; break;
                case "Azul": rAtual = 0; gAtual = 0; bAtual = 255; break;
                case "Roxo": rAtual = 128; gAtual = 0; bAtual = 128; break;
                case "Marrom": rAtual = 160; gAtual = 82; bAtual = 45; break;
                case "Cinza": rAtual = 192; gAtual = 192; bAtual = 192; break;
                case "Preto": rAtual = 0; gAtual = 0; bAtual = 0; break;
                case "Branco": rAtual = 255; gAtual = 255; bAtual = 255; break;
                case "Rosa": rAtual = 255; gAtual = 192; bAtual = 203; break;
                case "Ciano": rAtual = 0; gAtual = 255; bAtual = 255; break;
            }

            pnlCorSelecionada.BackColor = Color.FromArgb(rAtual, gAtual, bAtual);
        }
        private void CmbCorGeral_SelectedIndexChanged(object sender, EventArgs e)
        {
            int r = 255, g = 255, b = 255;

            switch (cmbCorGeral.SelectedItem.ToString())
            {
                case "Vermelho": r = 255; g = 0; b = 0; break;
                case "Azul": r = 0; g = 0; b = 255; break;
                case "Verde": r = 0; g = 128; b = 0; break;
                case "Amarelo": r = 255; g = 255; b = 0; break;
                case "Roxo": r = 128; g = 0; b = 128; break;
                case "Laranja": r = 255; g = 165; b = 0; break;
                case "Preto": r = 0; g = 0; b = 0; break;
                case "Branco": r = 255; g = 255; b = 255; break;
                case "Cinza": r = 192; g = 192; b = 192; break;
                case "Ciano": r = 0; g = 255; b = 255; break;
            }

            for (int i = 0; i < fundosFaces.Length; i++)
                fundosFaces[i] = fundo(r, g, b);

            pnlDesenho.Invalidate();
        }
        private void TbTransformacao_Scroll(object sender, EventArgs e)
        {
            offsetX = tbTranslacaoX.Value;
            offsetY = tbTranslacaoY.Value;
            anguloRotacao = tbRotacao.Value;
            escalaAtual = tbEscala.Value / 100f;

            lblValorTX.Text = offsetX.ToString();
            lblValorTY.Text = offsetY.ToString();
            lblValorRot.Text = anguloRotacao.ToString("0") + "°";
            lblValorEsc.Text = tbEscala.Value.ToString() + "%";

            pnlDesenho.Invalidate();
        }
        private void BtnResetar_Click(object sender, EventArgs e)
        {
            tbTranslacaoX.Value = 0;
            tbTranslacaoY.Value = 0;
            tbRotacao.Value = 0;
            tbEscala.Value = 100;

            TbTransformacao_Scroll(sender, e);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            MontarInterface();
            for (int i = 0; i < fundosFaces.Length; i++)
                fundosFaces[i] = fundo(255, 255, 255);
        }
    }
}