# Prompt para LLM: Novo design do portfólio em Blazor

## Contexto do projeto

Este é um sistema web de portfólio pessoal desenvolvido em .NET 10 + Blazor. A aplicação exibe um site profissional com as seguintes seções:

- Hero / apresentação inicial
- Sobre
- Experiência profissional
- Projetos
- Contato
- Painel administrativo para editar conteúdo

Estrutura relevante do projeto:

- `Components/Pages/Home.razor` — landing page principal
- `Data/PortfolioData.cs` — modelo dos dados do portfólio
- `Components/Pages/Admin.razor` — painel de administração
- `Program.cs` — configuração da aplicação
- `Services/PortfolioService.cs` — serviço que lê e salva os dados

A aplicação usa dados dinâmicos, então o novo design deve respeitar a estrutura já existente e preservar a possibilidade de editar conteúdo pela administração.

## Objetivo

Crie um novo visual para o sistema, modernizando o design do portfólio sem perder a funcionalidade atual. O design deve parecer premium, profissional, elegante e voltado para um perfil de desenvolvedor/criador digital.

## Regras obrigatórias

1. Preservar a semântica do conteúdo atual: nome, cargo, resumo, experiência, projetos, contato e links.
2. Manter compatibilidade com os dados do modelo `PortfolioData`.
3. O layout deve ser responsivo para desktop, tablet e mobile.
4. O design deve ter aparência moderna, sofisticada e tecnológica.
5. Priorizar legibilidade, impacto visual e boa hierarquia tipográfica.
6. Não quebrar a navegação por âncoras (`#sobre`, `#experiencias`, `#projetos`, `#contato`).
7. Manter consistência com a identidade de um portfólio profissional para desenvolvedor.
8. Considerar estilo premium, com máxima clareza visual e sensação de qualidade.
9. O painel administrativo deve continuar funcionando e sendo visualmente coerente com o novo design.
10. Evitar visual genérico ou clichê de “theme dark da moda” sem estratégia visual clara.

## Direção visual desejada

- Visual premium e minimalista
- Forte uso de hierarquia visual
- Paleta elegante com contraste bem pensado
- Tipografia moderna e forte
- Espaçamento generoso
- Cards e blocos com microinterações discretas
- Motif tecnológico sem ficar excessivamente “futurista”
- Fachada elegante e confiável, com foco em credibilidade profissional

## Sugestões de estilos que podem ser explorados

- Dark mode sofisticado com acentos em azul, violeta, verde ou cobre
- Visual com glassmorphism sutil ou superfícies elevadas
- Layout com hero de impacto e estatísticas discretas
- Seções em blocos bem delimitados com espaçamento editorial
- Gradientes discretos, não saturados demais
- Elementos de UI limpos, sem excesso de ornamentação

## Entregáveis esperados

Crie uma proposta de redesign completa contendo:

1. Nome da direção visual
2. Paleta de cores recomendada
3. Tipografia sugerida
4. Estrutura da página principal
5. Wireframe textual das seções
6. Sugestões de componentes reutilizáveis
7. Ideia de microinterações
8. Sugestões de CSS variables para facilitar manutenção
9. Descrição de como esse design respeita os dados do `PortfolioData`
10. Observações sobre acessibilidade e responsividade

## Saída desejada do LLM

O LLM deve responder como um designer de produto e UX/UI, com uma proposta clara, bem estruturada e visualmente inspirada. Deve indicar o que seria alterado no HTML/CSS do sistema atual para tornar o portfólio mais profissional e impactante.

Se for necessário, o LLM pode propor um conceito de redesign em Blazor com estrutura semelhante a:

- `header`/navegação elegante
- `hero` com mensagem forte e CTA
- `cards` para experiência e projetos
- `layout editorial` para conteúdo
- `footer` / área de contato com destaque

## Prompt final para copiar

"Você é um designer de interfaces e UX/UI sênior especializado em portfólios profissionais para desenvolvedores. Preciso que você proponha um novo design para este sistema, que é um portfólio em .NET 10 + Blazor.

Contexto do projeto:
- App de portfólio pessoal
- Landing page principal com seções de sobre, experiência, projetos e contato
- Painel administrativo para alterar conteúdo
- Dados dinâmicos vindos de PortfolioData
- Projeto em Blazor com Razor Components

Requisitos:
- Manter os dados e estrutura semântica do portfólio
- Preservar funcionalidade do painel admin
- Criar um visual moderno, premium, profissional e elegante
- Responsivo para mobile, tablet e desktop
- Foco em desenvolver um portfólio para um perfil de tecnologia/produto
- Evitar design genérico ou excessivamente carregado

Objetivo:
Crie uma proposta completa de redesign visual com paleta, tipografia, espaçamento, estrutura da página e sugestões de componentes.

Entregue:
1. Nome da direção visual
2. Paleta de cores
3. Tipografia
4. Estrutura visual da homepage
5. Wireframe textual
6. Componentes e blocos principais
7. Microinterações
8. Sugestões de CSS variables
9. Considerações de acessibilidade e responsividade
10. Como esse redesign se integra ao modelo atual de dados

Seja específico, criativo e técnico. Sugira um visual que aumente a percepção de qualidade e profissionalismo do projeto."

## Observação final

Use esse arquivo como base para um novo redesign no projeto. Se quiser, posso também transformar essa proposta em uma implementação real de CSS/Blazor com um novo layout visual completo.
