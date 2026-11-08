const mapas = new Map();

function escaparHtml(texto) 
{
    return String(texto).replace(/[&<>"']/g, caracter => 
    ({
        '&': '&amp;',
        '<': '&lt;',
        '>': '&gt;',
        '"': '&quot;',
        "'": '&#39;'
    })[caracter]);
}

function asegurarMapa(idMapa) 
{
    if(typeof L === 'undefined') 
        throw new Error('No se pudo cargar Leaflet. Comprueba tu conexión a internet para ver el mapa de OpenStreetMap.');

    let instancia = mapas.get(idMapa);

    if(instancia) 
        return instancia;

    const mapa = L.map(idMapa, 
    {
        zoomControl: true,
        scrollWheelZoom: true
    }).setView([-38.5, -63.0], 4);

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', 
    {
        maxZoom: 19,
        attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>'
    }).addTo(mapa);

    L.control.scale({ imperial: false }).addTo(mapa);

    instancia = 
    {
        mapa,
        marcadores: null,
        ruta: null
    };

    mapas.set(idMapa, instancia);

    return instancia;
}

function limpiarCapas(instancia) 
{
    if(instancia.marcadores) 
    {
        instancia.marcadores.remove();
        instancia.marcadores = null;
    }

    if(instancia.ruta) 
    {
        instancia.ruta.remove();
        instancia.ruta = null;
    }
}

export function mostrarMapa(idMapa, ciudades, ruta, trazado) 
{
    const instancia = asegurarMapa(idMapa);
    const mapa = instancia.mapa;

    ciudades = ciudades ?? [];
    ruta = ruta ?? [];

    limpiarCapas(instancia);

    instancia.marcadores = L.layerGroup().addTo(mapa);

    const orden = new Map();
    ruta.forEach((indice, posicion) => 
    {
        if(!orden.has(indice)) 
            orden.set(indice, posicion);
    });

    ciudades.forEach((ciudad, indice) => {
        const posicion = orden.get(indice);
        const numero = posicion === undefined ? '' : `${posicion + 1}. `;
        const nombre = escaparHtml(ciudad.nombre);

        const marcador = L.circleMarker([ciudad.lat, ciudad.lon], 
        {
            radius: 8,
            color: '#1d4ed8',
            weight: 2,
            fillColor: '#60a5fa',
            fillOpacity: 0.95
        });

        marcador.bindPopup(`<strong>${numero}${nombre}</strong>`);

        if(posicion !== undefined) 
        {
            marcador.bindTooltip(`${posicion + 1}. ${nombre}`, 
            {
                permanent: false,
                direction: 'top'
            });
        }

        marcador.addTo(instancia.marcadores);
    });

    if(ruta.length >= 2) 
    {
        const coordenadas = trazado && trazado.length >= 2 ? trazado : ruta.map(indice => [ciudades[indice].lat, ciudades[indice].lon]);
        instancia.ruta = L.polyline(coordenadas, 
        {
            color: '#db2777',
            weight: 4,
            opacity: 0.85,
            lineJoin: 'round'
        }).addTo(mapa);

        mapa.fitBounds(instancia.ruta.getBounds().pad(0.18));
    } 
    else if(ciudades.length === 1) 
        mapa.setView([ciudades[0].lat, ciudades[0].lon], 10);

    else if(ciudades.length > 1) 
    {
        const limites = L.latLngBounds(ciudades.map(ciudad => [ciudad.lat, ciudad.lon]));
        mapa.fitBounds(limites.pad(0.25));
    }
    else 
        mapa.setView([-38.5, -63.0], 4);

    setTimeout(() => mapa.invalidateSize(), 50);
}

export function destruirMapa(idMapa) 
{
    const instancia = mapas.get(idMapa);
    if(!instancia)
        return;

    limpiarCapas(instancia);
    instancia.mapa.remove();
    mapas.delete(idMapa);
}
