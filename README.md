## BridgeDim BR - Pré-dimensionamento de Pontes de Concreto Armado (NBR)

Ferramenta em Python para pré-dimensionamento simplificado de elementos usuais de pontes de concreto armado conforme práticas correntes no Brasil e parâmetros inspirados na NBR 6118 (concreto armado) e NBR 7188 (cargas móveis). Objetivo: estudo preliminar, não substitui projeto executivo nem verificação completa de ELU/ELS.

### Avisos e escopo
- **Uso**: pré-dimensionamentos e estimativas iniciais.
- **Normas**: parâmetros e fórmulas simplificados e parametrizáveis; não cobre todos os detalhes normativos.
- **Responsabilidade**: o usuário deve validar resultados com um engenheiro habilitado e verificação normativa completa.

### Requisitos
- Python 3.9+

### Instalação
Sem dependências externas. Basta clonar/copiar e executar:

```bash
python /workspace/main.py --help
```

### Uso rápido

- Slab (laje unidirecional, faixa de 1,00 m):
```bash
python /workspace/main.py slab \
  --span 8.0 \
  --fck 30 \
  --fyk 500 \
  --live_kN_m2 5.0 \
  --pav_thk_m 0.08
```

- Viga + laje (viga T simplificada, vão biapoiado):
```bash
python /workspace/main.py girder \
  --span 20.0 \
  --spacing 2.8 \
  --num_girders 6 \
  --slab_thk_m 0.20 \
  --fck 35 \
  --fyk 500 \
  --live_kN_m2 5.0 
```

### Principais hipóteses (ajustáveis por CLI)
- Combinação ELU: `w_sd = gamma_g * G_k + gamma_q * phi * Q_k`
- Pesos específicos: concreto 25 kN/m³, asfalto 23 kN/m³ (ajustáveis via parâmetros de espessura)
- Impacto (dinâmica/coef. amplificação): `phi = 1.30` (parâmetro de entrada)
- Momento fletor máximo em viga simplesmente apoiada: `M = w * L^2 / 8`
- Esforço cortante máximo: `V = w * L / 2`
- Alavanca interna: `z ≈ 0.9 * d`
- Aço CA-50: `f_yk = 500 MPa`, `γ_s = 1.15` (padrão)
- Concreto: `α_cc = 0.85`, `γ_c = 1.40` (padrão); `f_ctm = 0.3 * f_ck^(2/3)`
- Armadura mínima em lajes: `ρ_min = max(0.0015, 0.26 * f_ctm / f_yk)`

### Comandos
Veja todas as opções:
```bash
python /workspace/main.py --help
python /workspace/main.py slab --help
python /workspace/main.py girder --help
```

### Resultados
O CLI reporta dimensões preliminares, momentos, esforços cortantes, áreas de aço e sugestões de diâmetro/espaçamento por metro de laje ou por viga. Ajuste coberturas, diâmetros e espaçamentos conforme prática executiva e controle de fissuração.

### Limitações
- Não cobre efeitos de segunda ordem, estados de serviço, distribuição transversal detalhada, fadiga nem combinações completas de ações (NBR 8681).
- Modelo de carga móvel simplificado em carga distribuída equivalente por m². Para veículos específicos da NBR 7188, ajuste `live_kN_m2` ou acrescente cargas concentradas por sua conta.

### Licença
MIT

# CodeMestre-