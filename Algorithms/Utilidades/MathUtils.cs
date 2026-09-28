namespace traveling_salesman_problem_wasm.Algorithms.Utilidades;

internal static class MathUtils

{
    public static int  Factorial(int numerico)
    {
        if(numerico == 0)
            return 1;
            
        else
            return numerico * Factorial(numerico-1); 
    }
}
