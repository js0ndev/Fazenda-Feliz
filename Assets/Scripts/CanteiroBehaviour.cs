using System.Collections;
using UnityEngine;

// Enum que define todos os estados possíveis de um canteiro.
// Um enum é basicamente uma lista de constantes nomeadas — muito melhor do que
// usar números mágicos (0, 1, 2...) espalhados pelo código.
public enum EstadoCanteiro
{
    Vazio,       // Terra intocada
    Preparado,   // Terra arada com a enxada
    Semeado,     // Semente plantada, aguardando
    Brotando,    // Meio do caminho do crescimento
    Pronto       // Pronto para colheita
}

public class CanteiroBehaviour : MonoBehaviour
{
    // -------------------------------------------------------------------------
    // CONFIGURAÇÃO DOS SPRITES
    // -------------------------------------------------------------------------

    // SerializeField expõe o campo no Inspector da Unity sem precisar ser public.
    // Boa prática: mantenha campos private/protected e use SerializeField quando
    // precisar configurar pelo Inspector.
    [Header("Sprites de cada estado")]
    [SerializeField] private GameObject spriteVazio;
    [SerializeField] private GameObject spritePreparado;
    [SerializeField] private GameObject spriteSemeado;
    [SerializeField] private GameObject spriteBrotando;
    [SerializeField] private GameObject spritePronto;

    // -------------------------------------------------------------------------
    // CONFIGURAÇÃO DO TEMPO DE CRESCIMENTO
    // -------------------------------------------------------------------------

    [Header("Tempo de crescimento (em segundos)")]
    [SerializeField] private float tempoParaBrotar  = 10f; // Semeado  → Brotando
    [SerializeField] private float tempoParaMadurar = 10f; // Brotando → Pronto

    // -------------------------------------------------------------------------
    // ESTADO ATUAL
    // -------------------------------------------------------------------------

    // Propriedade pública somente-leitura: outros scripts podem LER o estado,
    // mas só este script pode ALTERAR. Encapsulamento clássico de C#.
    public EstadoCanteiro EstadoAtual { get; private set; }

    // Guardamos a referência da coroutine para poder cancelá-la se necessário
    // (ex: uma geada que mata a planta no futuro).
    private Coroutine _coroutineCrescimento;

    // -------------------------------------------------------------------------
    // INICIALIZAÇÃO
    // -------------------------------------------------------------------------

    private void Start()
    {
        // Garante que o canteiro começa no estado correto ao iniciar a cena.
        AtualizarVisual(EstadoCanteiro.Vazio);
    }

    // -------------------------------------------------------------------------
    // AÇÕES PÚBLICAS — chamadas por outros scripts (ex: PlantacaoManager)
    // -------------------------------------------------------------------------

    // Tenta preparar a terra. Retorna true se a ação foi válida.
    // O retorno bool é útil para o chamador saber se algo aconteceu
    // (ex: tocar um som de erro se retornar false).
    public bool PrepararTerra()
    {
        if (EstadoAtual != EstadoCanteiro.Vazio)
        {
            Debug.Log($"[Canteiro] Não é possível preparar: estado atual é {EstadoAtual}");
            return false;
        }

        AtualizarVisual(EstadoCanteiro.Preparado);
        return true;
    }

    public bool PlantarSemente()
    {
        if (EstadoAtual != EstadoCanteiro.Preparado)
        {
            Debug.Log($"[Canteiro] Não é possível plantar: estado atual é {EstadoAtual}");
            return false;
        }

        AtualizarVisual(EstadoCanteiro.Semeado);

        // Inicia o ciclo de crescimento usando Coroutine.
        // Coroutines são funções que podem ser "pausadas" com yield return
        // sem travar o jogo — essencial para timers e animações sequenciais.
        _coroutineCrescimento = StartCoroutine(CicloDeCrescimento());
        return true;
    }

    public bool Colher()
    {
        if (EstadoAtual != EstadoCanteiro.Pronto)
        {
            Debug.Log($"[Canteiro] Não é possível colher: estado atual é {EstadoAtual}");
            return false;
        }

        AtualizarVisual(EstadoCanteiro.Vazio);

        // Aqui futuramente: adicionar item ao inventário do jogador.
        Debug.Log("[Canteiro] Colhido! Item adicionado ao inventário (TODO).");
        return true;
    }

    // -------------------------------------------------------------------------
    // COROUTINE DE CRESCIMENTO
    // -------------------------------------------------------------------------

    // IEnumerator é obrigatório para Coroutines na Unity.
    // "yield return new WaitForSeconds(X)" pausa a execução por X segundos
    // sem bloquear o restante do jogo — é como um await Task.Delay em .NET.
    private IEnumerator CicloDeCrescimento()
    {
        // Fase 1: aguarda para brotar
        yield return new WaitForSeconds(tempoParaBrotar);
        AtualizarVisual(EstadoCanteiro.Brotando);

        // Fase 2: aguarda para ficar pronto
        yield return new WaitForSeconds(tempoParaMadurar);
        AtualizarVisual(EstadoCanteiro.Pronto);

        Debug.Log("[Canteiro] Planta pronta para colheita!");
    }

    // -------------------------------------------------------------------------
    // CONTROLE DE VISUAL (privado — detalhe de implementação)
    // -------------------------------------------------------------------------

    private void AtualizarVisual(EstadoCanteiro novoEstado)
    {
        EstadoAtual = novoEstado;

        // Desativa todos os sprites primeiro, depois ativa apenas o correto.
        // Esse padrão evita condições redundantes e é fácil de expandir.
        spriteVazio.SetActive(false);
        spritePreparado.SetActive(false);
        spriteSemeado.SetActive(false);
        spriteBrotando.SetActive(false);
        spritePronto.SetActive(false);

        // Switch expression — sintaxe moderna do C# 8+, mais limpa que switch/case
        // para mapeamentos simples como esse.
        GameObject spriteAtivo = novoEstado switch
        {
            EstadoCanteiro.Vazio     => spriteVazio,
            EstadoCanteiro.Preparado => spritePreparado,
            EstadoCanteiro.Semeado   => spriteSemeado,
            EstadoCanteiro.Brotando  => spriteBrotando,
            EstadoCanteiro.Pronto    => spritePronto,
            _                        => spriteVazio // fallback de segurança
        };

        spriteAtivo.SetActive(true);

        Debug.Log($"[Canteiro] Estado atualizado para: {novoEstado}");
    }
}