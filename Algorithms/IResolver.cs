using traveling_salesman_problem_wasm.Models;

namespace traveling_salesman_problem_wasm.Algorithms;

internal interface IResolver
{
    SolucionTSP? Resolver();
}
