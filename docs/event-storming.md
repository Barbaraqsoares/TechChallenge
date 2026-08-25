# Event Storming — FIAP Cloud Games (FCG)

Documentação DDD do Tech Challenge — Fase 1.

O Event Storming é a oficina que descobre como o negócio funciona **por meio dos
eventos**, começando pelos acontecimentos e não por telas ou tabelas. Este documento
registra o resultado da modelagem dos dois fluxos exigidos pelo desafio — **criação de
usuários** e **criação de jogos** — mais o fluxo de **aquisição**, que é o coração da
plataforma.

---

## Legenda

Notação da disciplina (Aula 06 — Event Storming):

| Elemento | Cor | O que é | Como se escreve |
|---|---|---|---|
| **Evento de domínio** | 🟧 Laranja | Algo que **já aconteceu** | Passado — "Usuário cadastrado" |
| **Comando** | 🟦 Azul | Ação que **causa** o evento | Imperativo — "Cadastrar usuário" |
| **Ator** | 🟨 Amarelo | Quem dispara o comando | Pela função — "Administrador" |
| **Política** | 🟪 Roxo | Regra que **reage** a um evento e dispara outro comando | "Quando… então…" |
| **Modelo de leitura** | 🟩 Verde | Tela/consulta vista **antes** de um comando | "Catálogo de jogos" |
| **Ponto de atenção** | 🔴 Rosa | Dúvida, risco ou gargalo a investigar | Pergunta aberta |
| **Evento pivotal** | ⬛ Linha vertical | Marca troca de fase — indica **contexto delimitado** | — |
| **Agregado** | ⬜ Agrupamento | Objeto central que reúne comandos e eventos | — |

---

## Domínio e subdomínios

O domínio da FCG é a **venda de jogos digitais e gestão de partidas online**. Dividido
segundo a regra de decisão da Aula 01:

| Subdomínio | Tipo | Por quê |
|---|---|---|
| **Catálogo e Biblioteca de Jogos** | 🔴 Core | É o que diferencia a plataforma — vender jogos e manter a biblioteca do jogador |
| **Identidade e Acesso** | 🟡 Genérico | Todo mundo no mercado tem igual; complexo, mas não diferencia |
| **Cadastro de Usuários** | 🟢 Suporte | Lógica simples, apoia o principal |

> Atenção ao contexto: autenticação é **genérica** para a FCG, mas seria o **core** de
> uma empresa como a Auth0. O tipo depende de quem está olhando.

---

## Fluxo 1 — Criação de usuários

### Linha do tempo

```mermaid
flowchart LR
    A1(["👤 Visitante"]):::ator
    C1["Cadastrar usuário"]:::comando
    E1["Usuário cadastrado"]:::evento
    P1["Quando usuário cadastrado<br/>→ atribuir perfil Cliente"]:::politica
    E2["Perfil Cliente atribuído"]:::evento

    A1 --> C1 --> E1 --> P1 --> E2

    classDef evento fill:#FFA726,stroke:#E65100,color:#000
    classDef comando fill:#42A5F5,stroke:#0D47A1,color:#000
    classDef ator fill:#FFEE58,stroke:#F57F17,color:#000
    classDef politica fill:#B39DDB,stroke:#4527A0,color:#000
```

### Caminho ideal e exceções

```mermaid
flowchart TD
    C1["Cadastrar usuário"]:::comando

    C1 --> V{Validações do domínio}
    V -->|nome, e-mail e senha válidos| E1["Usuário cadastrado"]:::evento
    V -->|formato inválido| X1["Cadastro rejeitado:<br/>e-mail inválido"]:::evento
    V -->|menos de 8 caracteres,<br/>sem letra, número ou especial| X2["Cadastro rejeitado:<br/>senha fora da política"]:::evento
    V -->|e-mail já existe| X3["Cadastro rejeitado:<br/>e-mail duplicado"]:::evento

    E1 --> P1["Quando usuário cadastrado<br/>→ gerar hash da senha"]:::politica
    P1 --> E2["Senha protegida por hash"]:::evento

    classDef evento fill:#FFA726,stroke:#E65100,color:#000
    classDef comando fill:#42A5F5,stroke:#0D47A1,color:#000
    classDef politica fill:#B39DDB,stroke:#4527A0,color:#000
```

> As exceções são eventos de domínio como quaisquer outros — no Event Storming elas
> entram na linha do tempo, não são "erros técnicos" escondidos.

### Autenticação (evento pivotal)

```mermaid
flowchart LR
    A1(["👤 Usuário"]):::ator
    C2["Autenticar usuário"]:::comando
    E3["Usuário autenticado"]:::evento
    P2["Quando usuário autenticado<br/>→ emitir token com o perfil"]:::politica
    E4["Token JWT emitido"]:::evento
    X4["Autenticação recusada"]:::evento

    A1 --> C2
    C2 -->|credenciais conferem| E3 --> P2 --> E4
    C2 -->|e-mail ou senha inválidos| X4

    classDef evento fill:#FFA726,stroke:#E65100,color:#000
    classDef comando fill:#42A5F5,stroke:#0D47A1,color:#000
    classDef ator fill:#FFEE58,stroke:#F57F17,color:#000
    classDef politica fill:#B39DDB,stroke:#4527A0,color:#000
```

**⬛ Evento pivotal: "Usuário autenticado"**

Marca a fronteira entre o contexto de **Identidade e Acesso** e todo o restante da
plataforma. Antes dele o visitante é anônimo; depois, todo comando carrega um perfil
que decide o que ele pode fazer.

### Administração de usuários

```mermaid
flowchart LR
    A2(["👤 Administrador"]):::ator
    L1[["Lista de usuários"]]:::leitura
    C3["Alterar perfil do usuário"]:::comando
    C4["Remover usuário"]:::comando
    E5["Perfil do usuário alterado"]:::evento
    E6["Usuário removido"]:::evento
    P3["Quando usuário removido<br/>→ remover sua biblioteca"]:::politica
    E7["Biblioteca removida"]:::evento

    A2 --> L1 --> C3 --> E5
    L1 --> C4 --> E6 --> P3 --> E7

    classDef evento fill:#FFA726,stroke:#E65100,color:#000
    classDef comando fill:#42A5F5,stroke:#0D47A1,color:#000
    classDef ator fill:#FFEE58,stroke:#F57F17,color:#000
    classDef politica fill:#B39DDB,stroke:#4527A0,color:#000
    classDef leitura fill:#A5D6A7,stroke:#1B5E20,color:#000
```

---

## Fluxo 2 — Criação de jogos

```mermaid
flowchart TD
    A2(["👤 Administrador"]):::ator
    C5["Cadastrar jogo"]:::comando

    A2 --> C5
    C5 --> V{Validações do domínio}
    V -->|título preenchido e<br/>preço não negativo| E8["Jogo cadastrado"]:::evento
    V -->|título vazio| X5["Cadastro rejeitado:<br/>título obrigatório"]:::evento
    V -->|preço negativo| X6["Cadastro rejeitado:<br/>preço inválido"]:::evento

    E8 --> P4["Quando jogo cadastrado<br/>→ publicar no catálogo sem promoção"]:::politica
    P4 --> E9["Jogo disponível no catálogo"]:::evento

    classDef evento fill:#FFA726,stroke:#E65100,color:#000
    classDef comando fill:#42A5F5,stroke:#0D47A1,color:#000
    classDef ator fill:#FFEE58,stroke:#F57F17,color:#000
    classDef politica fill:#B39DDB,stroke:#4527A0,color:#000
```

### Manutenção e promoções

```mermaid
flowchart LR
    A2(["👤 Administrador"]):::ator
    L2[["Catálogo de jogos"]]:::leitura
    C6["Atualizar jogo"]:::comando
    C7["Aplicar promoção"]:::comando
    C8["Encerrar promoção"]:::comando
    C9["Remover jogo"]:::comando

    E10["Jogo atualizado"]:::evento
    E11["Promoção aplicada"]:::evento
    E12["Promoção encerrada"]:::evento
    E13["Jogo removido do catálogo"]:::evento
    X7["Promoção rejeitada:<br/>desconto fora de 0–90%"]:::evento

    P5["Quando promoção aplicada<br/>→ recalcular preço atual"]:::politica
    E14["Preço promocional vigente"]:::evento

    A2 --> L2
    L2 --> C6 --> E10
    L2 --> C7
    C7 -->|desconto entre 0% e 90%| E11 --> P5 --> E14
    C7 -->|fora do limite| X7
    L2 --> C8 --> E12
    L2 --> C9 --> E13

    classDef evento fill:#FFA726,stroke:#E65100,color:#000
    classDef comando fill:#42A5F5,stroke:#0D47A1,color:#000
    classDef ator fill:#FFEE58,stroke:#F57F17,color:#000
    classDef politica fill:#B39DDB,stroke:#4527A0,color:#000
    classDef leitura fill:#A5D6A7,stroke:#1B5E20,color:#000
```

---

## Fluxo 3 — Aquisição de jogo (o core)

```mermaid
flowchart TD
    A1(["👤 Usuário"]):::ator
    L2[["Catálogo de jogos"]]:::leitura
    L3[["Lista de promoções"]]:::leitura
    C10["Adquirir jogo"]:::comando

    A1 --> L2 --> C10
    A1 --> L3 --> C10

    C10 --> V{Já possui o jogo?}
    V -->|não| E15["Jogo adquirido"]:::evento
    V -->|sim| X8["Aquisição rejeitada:<br/>jogo já na biblioteca"]:::evento

    E15 --> P6["Quando jogo adquirido<br/>→ congelar o preço pago"]:::politica
    P6 --> E16["Preço pago registrado"]:::evento
    E16 --> E17["Biblioteca do usuário atualizada"]:::evento

    classDef evento fill:#FFA726,stroke:#E65100,color:#000
    classDef comando fill:#42A5F5,stroke:#0D47A1,color:#000
    classDef ator fill:#FFEE58,stroke:#F57F17,color:#000
    classDef politica fill:#B39DDB,stroke:#4527A0,color:#000
    classDef leitura fill:#A5D6A7,stroke:#1B5E20,color:#000
```

**⬛ Evento pivotal: "Jogo adquirido"**

Separa o contexto de **Catálogo** (o jogo como produto à venda) do contexto de
**Biblioteca** (o jogo como item que pertence a alguém). É o momento em que o preço
deixa de ser "de tabela" e passa a ser "o que foi pago".

---

## Pontos de atenção

Levantados durante a modelagem — riscos e decisões conscientes desta fase:

| 🔴 Ponto de atenção | Situação nesta fase |
|---|---|
| Não existe pagamento — a aquisição é imediata | Fora do escopo da Fase 1; o preço é registrado, mas nada é cobrado |
| O administrador inicial nasce com senha padrão | Necessário para quebrar o impasse (só admin cadastra jogo); precisa ser trocada |
| Não há confirmação de e-mail | O cadastro é aceito sem verificar se o e-mail existe de fato |
| Não há refresh token | Expirado o token, é preciso autenticar de novo |
| Remover usuário apaga a biblioteca | Perde-se o histórico de compras — reavaliar quando houver pagamento |
| Remover jogo é bloqueado se alguém já comprou | Protege o histórico da biblioteca |
| Promoção não tem vigência por data | O desconto vale até alguém encerrá-lo manualmente |

---

## Agregados

Agrupando comandos e eventos em torno do objeto que protege a consistência:

```mermaid
flowchart TB
    subgraph AG1["🔷 Agregado: Usuário (raiz)"]
        U1["Usuario"]
        U2["Email (objeto de valor)"]
        U3["Senha (objeto de valor)"]
        U4["UsuarioJogo — itens da biblioteca"]
        U1 --- U2
        U1 --- U3
        U1 --- U4
    end

    subgraph AG2["🔷 Agregado: Jogo (raiz)"]
        J1["Jogo"]
        J2["Preço e percentual de desconto"]
        J1 --- J2
    end

    AG1 -.->|referencia por Id| AG2

    style AG1 fill:#FFF8E1,stroke:#F57F17
    style AG2 fill:#FFF8E1,stroke:#F57F17
```

**Consistência forçada:** a biblioteca só muda por `Usuario.AdquirirJogo()`. Nenhum
objeto externo adiciona um item diretamente na coleção — pode apenas **solicitar** que
o agregado o faça, e é aí que a regra "não pode comprar duas vezes" é aplicada.

| Bloco | Pergunta-chave | No projeto |
|---|---|---|
| **Entidade** | Quem é? | `Usuario`, `Jogo`, `UsuarioJogo` |
| **Objeto de Valor** | Quanto vale / como é? | `Email`, `Senha` |
| **Agregado** | Qual conjunto se protege? | `Usuario` + biblioteca |

---

## Contextos delimitados

Os eventos pivotais revelaram três fronteiras:

```mermaid
flowchart LR
    subgraph CTX1["Identidade e Acesso"]
        I1["Cadastrar usuário"]
        I2["Autenticar"]
        I3["Emitir token"]
        I4["Alterar perfil"]
    end

    subgraph CTX2["Catálogo"]
        C1["Cadastrar jogo"]
        C2["Atualizar jogo"]
        C3["Aplicar promoção"]
    end

    subgraph CTX3["Biblioteca"]
        B1["Adquirir jogo"]
        B2["Consultar biblioteca"]
    end

    CTX1 -->|"identidade do usuário"| CTX3
    CTX2 -->|"preço vigente do jogo"| CTX3

    style CTX1 fill:#E3F2FD,stroke:#0D47A1
    style CTX2 fill:#F3E5F5,stroke:#4A148C
    style CTX3 fill:#E8F5E9,stroke:#1B5E20
```

Nesta fase, como o desafio pede um **monolito**, os três contextos convivem no mesmo
processo e no mesmo banco — mas já separados em pastas e agregados, o que permite
extraí-los em serviços independentes nas próximas fases.

**Padrão de integração:** Catálogo é *upstream* da Biblioteca (fornece o preço vigente);
Identidade é *upstream* de ambos (fornece quem está agindo).

---

## Linguagem ubíqua

O dicionário do projeto — os mesmos termos na conversa, na documentação e no código:

| Termo | Significado |
|---|---|
| **Usuário** | Pessoa cadastrada na plataforma; tem um perfil |
| **Perfil** | Nível de acesso: `Cliente` ou `Admin` |
| **Cliente** | Usuário que acessa a plataforma e sua biblioteca |
| **Administrador** | Usuário que cadastra jogos, administra usuários e cria promoções |
| **Jogo** | Item do catálogo, com preço de tabela |
| **Catálogo** | Conjunto de todos os jogos disponíveis |
| **Promoção** | Desconto percentual vigente sobre o preço de tabela |
| **Preço de tabela** | Valor cadastrado do jogo, sem desconto |
| **Preço atual** | Valor cobrado hoje, já com a promoção aplicada |
| **Preço pago** | Valor efetivamente pago na aquisição — não muda depois |
| **Adquirir** | Ato de incluir um jogo na biblioteca do usuário |
| **Biblioteca** | Conjunto de jogos que um usuário adquiriu |

> **Termo ambíguo resolvido:** "preço" significava três coisas diferentes — o cadastrado,
> o promocional e o que o usuário pagou. Separar em **preço de tabela**, **preço atual** e
> **preço pago** eliminou a ambiguidade, e os três nomes aparecem no código.

---

## Rastreabilidade — do evento ao código

Cada evento modelado tem endereço na implementação:

| Evento de domínio | Onde vive |
|---|---|
| Usuário cadastrado | `Usuario.Criar()` |
| Cadastro rejeitado: e-mail inválido | `Email.Criar()` |
| Cadastro rejeitado: senha fora da política | `Senha.Criar()` |
| Cadastro rejeitado: e-mail duplicado | `AuthController.Registrar()` |
| Usuário autenticado / recusado | `Usuario.Autenticar()` |
| Token JWT emitido | `TokenService.GerarToken()` |
| Perfil do usuário alterado | `Usuario.AlterarPerfil()` |
| Jogo cadastrado / rejeitado | `Jogo.Criar()` |
| Jogo atualizado | `Jogo.Atualizar()` |
| Promoção aplicada / rejeitada | `Jogo.AplicarPromocao()` |
| Promoção encerrada | `Jogo.EncerrarPromocao()` |
| Preço promocional vigente | `Jogo.PrecoAtual()` |
| Jogo adquirido | `Usuario.AdquirirJogo()` |
| Aquisição rejeitada: já na biblioteca | `Usuario.JaPossui()` |
| Preço pago registrado | `UsuarioJogo.Criar()` |

---

## Resumo do workshop

Seguindo a ordem da Aula 06:

1. **Brainstorming de eventos** — 20 eventos de domínio levantados, incluindo as rejeições
2. **Linha do tempo** — organizados em caminho ideal + exceções
3. **Pontos de atenção** — 7 riscos registrados
4. **Eventos pivotais** — "Usuário autenticado" e "Jogo adquirido"
5. **Comandos e atores** — 10 comandos, 3 atores (Visitante, Usuário, Administrador)
6. **Políticas** — 6 automações que reagem a eventos
7. **Modelos de leitura** — catálogo, promoções, biblioteca, perfil, lista de usuários
8. **Sistemas externos** — nenhum nesta fase (pagamento entra em fases futuras)
9. **Agregados** — Usuário (com a biblioteca) e Jogo
10. **Contextos delimitados** — Identidade e Acesso, Catálogo e Biblioteca
