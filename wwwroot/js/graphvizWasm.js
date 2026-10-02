import { Graphviz } from 'https://cdn.jsdelivr.net/npm/@hpcc-js/wasm-graphviz@1/dist/index.js';

let graphvizInstance = null;

async function getInstance() 
{
    if (!graphvizInstance) 
        graphvizInstance = await Graphviz.load();

    return graphvizInstance;
}

export async function renderDot(dotSource) 
{
    const graphviz = await getInstance();
    console.log(dotSource);
    return graphviz.dot(dotSource); 
}
