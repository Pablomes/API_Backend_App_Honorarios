# API BACKEND APP PBL IVE

## Funcionamiento básico

El programa necesita acceso a una BBDD que tenga un serie de tablas correspondientes a los modelos en la carpeta *Models*.

Su función principal es la de calcular los valores correspondientes a las peticiones de la aplicación web, utilizando las fórmulas y los datos guardados en la BBDD.

## Tipos utilizados comunes

A continuación, la definición en TypeScript de tipos utilizados de forma recurrente en los endpoints de manera que se aligera la redacción de la siguiente sección. Su uso en C# es usando una conversión de JSON. Aquellos tipos que sean uniones de cadenas, se representan como enumeraciones.

```ts
export type UrbanisationUse = {
  id: number,
  name: string,
  greenArea: number | null,
  networkArea: number | null,
  unitPEMGreenArea: number | null,
    unitPEMNetworkArea: number | null,
    valid: boolean
}

export type CivilWorksUse = {
    id: number;
    type: string;
    area: number | null;
    unitPEM: number | null;
    totalPEM: number | null;
    valid: boolean;

    // METADATA
    totalPEMManual: boolean;
}

export type EdificationUse = {
  id: number;
    name: string;
    repeatedFloors: number | null;
    area: number | null;
    unitPEM: number | null;
    installationPEM: number | null;
    valid: boolean;
}

export type EdificationCalculationRequest = {
    type: "EDIF";
    uses: EdificationUse[];
    projectState: "ESPR" | "ANPR" | "PRBA" | "PREJ" | "PBEJ" | "OIOB" | "PBED" | "ATSU" | "ACPR";
}

export type CivilWorksCalculationRequest = {
    type: "OBCI";
    uses: CivilWorksUse[];
    projectState: "MEVA" | "ANPR" | "PRCO" | "PCOD" | "DOAT";
}

export type UrbanisationCalculationRequest = {
    type: "URBA";
    uses: UrbanisationUse[];
    projectState: "ANPR" | "PROY" | "DIOB" | "ESIA" | "ESIP";
}

export type AddonInfo = {
    id: string;
    enabledUses: boolean[];
}

export type HonorariosCalculationRequest = (EdificationCalculationRequest | CivilWorksCalculationRequest | UrbanisationCalculationRequest) & {
    selectedAddons: AddonInfo[];
}

export type AddonCalculationResponse = {
    id: string;
    values: number[];
}

export type HonorariosCalculationResponse = {
    projectCosts: number[];
    responses: AddonCalculationResponse[];
}

export type GenericDoc = {
    id: string;
    nombre: string;
    info: string | null;
    toggleUseCase: boolean;
    orderIdx: number;
}

export type EdificationDoc = GenericDoc;
export type CivilWorksDoc = GenericDoc;
export type UrbanisationDoc = GenericDoc;

export type GenericProject<TDoc extends GenericDoc> = {
    id: string;
    nombre: string;
    docs: TDoc[];
    orderIdx: number;
}

export type EdificationProject = GenericProject<EdificationDoc>;
export type CivilWorksProject = GenericProject<CivilWorksDoc>;
export type UrbanisationProject = GenericProject<UrbanisationDoc>;

export type DocRequest = {
    projectType: "EDIF" | "OBCI" | "URBA";
    actuationId: string;
    calculationRequest: HonorariosCalculationRequest;
}
```

## Endpoints

#### **POST** /honorarios/calculate

```
params: (HonorariosCalculationRequest)
```

Calcula el resultado de la petición dada y la devuelve como una ```HonorariosCalculationResponse```.

#### **GET** /honorarios/docs/{type}

```
type: string
```

Retorna una lista con todos los documentos de cierto tipo.

#### **GET** /honorarios/projects/{type}

```
type: string
```

Retorna una lista con todos los proyectos de cierto tipo.

#### **POST** /honorariosDoc

```
params: (DocRequest)
```

Calcula el resultado de la petición de cálculo dada dentro de la petición de documento, y utiliza esos datos para generar y guardar un proyecto en la base de datos y un blob. Retorna el documento como datos crudos.

#### **GET** /honorariosDoc/test

Genera un documento de prueba y lo retorna de la misma manera que el verdadero. No lo guarda.

#### **GET** /honorariosDoc/{cve}

```
cve: string
```

Recupera y retorna el documento con el CVE dado si existe.

## Procesamiento de datos

Para facilitar la iteración sobre las fórmulas de la base de datos, estas se guardan en formato de texto. Así, se facilita flexibilidad sin tener que escribirlas directamente sobre el código. Además, para evitar posibles comportamientos indebidos posibles si se utilizara inyección de código JavaScript, se ha utilizado un simple interpretador limitado que solamente permite una gramática muy reducida.

> [!NOTE]
> De igual forma que se pueden añadir variables al interprete, cualquier variable que no conozca la intentará buscar en la BBDD. Esto inclye buscar fórmulas y calcular valores de forma recursiva para dar con el resultado final.

### Gramática permitida

La gramática es la siguiente:

```
<Expression> ::= <Term> | <Term> + <Expression> | <Term> - <Expression>

<Term> ::= <Power> | <Power> * <Term> | <Power> / <Term>

<Power> ::= <Factor> | <Factor> ^ <Factor>

<Factor> ::= <Atom> | - <Factor>

<Atom> ::= <Variable> | <Constant> | ( <Atom> )

<Letter> ::= a - z | A - Z

<Digit> ::= 0 | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9

<Variable> ::= <letter> | <letter> <Variable>

<Num> ::= <Digit> | <Digit> <Num>

<Constant> ::= <Num> | <Num> . <Num>
```

> [!CAUTION]
> Cualquier expresión que no cumpla con la gramática provocará un crasheo del backend, o comportamientos indebidos. Esto se debe tener en consideración al introducir ecuaciones en la BBDD.