import { Graphviz } from 'https://cdn.jsdelivr.net/npm/@hpcc-js/wasm-graphviz@1/dist/index.js';

let graphvizInstance = null;

async function getInstance()
{
    if (!graphvizInstance)
        graphvizInstance = await Graphviz.load();

    return graphvizInstance;
}

function puntuarLayout(json)
{
    const g = JSON.parse(json);
    const nodos = new Map();

    for (const o of g.objects ?? [])
    {
        if (o.pos === undefined) continue;
        const [x, y] = o.pos.split(',').map(Number);
        nodos.set(o._gvid, { x, y, r: Math.max(parseFloat(o.width), parseFloat(o.height)) * 36 });
    }

    const lista = [...nodos.entries()];
    let penalizacion = 0;
    for (let a = 0; a < lista.length; a++)
        for (let b = a + 1; b < lista.length; b++)
        {
            const A = lista[a][1], B = lista[b][1];
            if (Math.hypot(A.x - B.x, A.y - B.y) < A.r + B.r + 20) penalizacion += 1;
        }

    const etiquetas = [];

    for (const e of g.edges ?? [])
    {
        const bezier = (e._draw_ ?? []).find(d => d.op === 'b');
        if (bezier)
        {
            const p = bezier.points, puntos = [];
            for (let i = 0; i + 3 < p.length; i += 3)
                for (let k = 0; k <= 12; k++)
                {
                    const t = k / 12, u = 1 - t;
                    puntos.push([
                        u ** 3 * p[i][0] + 3 * u * u * t * p[i + 1][0] + 3 * u * t * t * p[i + 2][0] + t ** 3 * p[i + 3][0],
                        u ** 3 * p[i][1] + 3 * u * u * t * p[i + 1][1] + 3 * u * t * t * p[i + 2][1] + t ** 3 * p[i + 3][1]]);
                }

            for (const [id, n] of nodos)
                if (id !== e.tail && id !== e.head && puntos.some(([x, y]) => Math.hypot(x - n.x, y - n.y) < n.r + 1))
                    penalizacion += 2;
        }

        if (e.lp !== undefined)
        {
            const [x, y] = e.lp.split(',').map(Number);
            etiquetas.push({ x, y, mw: (e.label ?? '').length * 3 + 2, mh: 8 });
        }
    }

    for (let a = 0; a < etiquetas.length; a++)
    {
        const A = etiquetas[a];

        for (let b = a + 1; b < etiquetas.length; b++)
        {
            const B = etiquetas[b];
            if (Math.abs(A.x - B.x) < A.mw + B.mw && Math.abs(A.y - B.y) < A.mh + B.mh) penalizacion += 1;
        }

        for (const n of nodos.values())
            if (Math.abs(A.x - n.x) < A.mw + n.r * 0.8 && Math.abs(A.y - n.y) < A.mh + n.r * 0.8) penalizacion += 1;
    }

    return penalizacion;
}

export async function renderDots(dots)
{
    const graphviz = await getInstance();
    const intentos = 24;
    let mejorSemilla = 1, mejorPuntaje = Infinity;

    for (let semilla = 1; semilla <= intentos; semilla++)
    {
        try
        {
            const candidato = dots[0].replace(/start=\d+/, `start=${semilla}`);
            const puntaje = puntuarLayout(graphviz.layout(candidato, 'json', 'neato'));

            if (puntaje < mejorPuntaje)
            {
                mejorPuntaje = puntaje;
                mejorSemilla = semilla;
            }

            if (puntaje === 0) break;
        }
        catch (e)
        {
            console.warn('No se pudo evaluar el layout', e);
            break;
        }
    }

    return dots.map(d => graphviz.layout(d.replace(/start=\d+/, `start=${mejorSemilla}`), 'svg', 'neato'));
}
