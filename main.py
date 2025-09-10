#!/usr/bin/env python3
import argparse
from typing import Tuple


# -----------------------------
# Materiais e coeficientes
# -----------------------------

def concrete_design_strength(fck_mpa: float, gamma_c: float = 1.40, alpha_cc: float = 0.85) -> float:
    fcd = alpha_cc * fck_mpa / gamma_c
    return fcd


def steel_design_strength(fyk_mpa: float, gamma_s: float = 1.15) -> float:
    fyd = fyk_mpa / gamma_s
    return fyd


def fctm_from_fck(fck_mpa: float) -> float:
    return 0.3 * (fck_mpa ** (2.0 / 3.0))


def rho_min_slab(fck_mpa: float, fyk_mpa: float) -> float:
    fctm = fctm_from_fck(fck_mpa)
    rho = max(0.0015, 0.26 * fctm / fyk_mpa)
    return rho


# -----------------------------
# Cargas e combinações
# -----------------------------

def uls_uniform_load_per_meter(
    span_m: float,
    width_m: float,
    slab_thk_m: float,
    wearing_thk_m: float,
    live_kN_m2: float,
    gamma_g: float = 1.35,
    gamma_q: float = 1.50,
    phi_impact: float = 1.30,
    unit_weight_conc_kN_m3: float = 25.0,
    unit_weight_asph_kN_m3: float = 23.0,
) -> Tuple[float, float, float]:
    gk_slab_kN_m2 = unit_weight_conc_kN_m3 * slab_thk_m
    gk_wearing_kN_m2 = unit_weight_asph_kN_m3 * wearing_thk_m
    gk_total_kN_m2 = gk_slab_kN_m2 + gk_wearing_kN_m2
    qk_equiv_kN_m2 = live_kN_m2
    wsd_kN_m2 = gamma_g * gk_total_kN_m2 + gamma_q * phi_impact * qk_equiv_kN_m2
    wsd_kN_m = wsd_kN_m2 * width_m
    return wsd_kN_m, gk_total_kN_m2 * width_m, phi_impact * qk_equiv_kN_m2 * width_m


# -----------------------------
# Laje maciça unidirecional (faixa de 1 m)
# -----------------------------

def design_slab(
    span_m: float,
    fck_mpa: float,
    fyk_mpa: float,
    live_kN_m2: float,
    slab_thk_m: float,
    wearing_thk_m: float,
    phi_impact: float = 1.30,
    cover_m: float = 0.035,
    bar_diam_mm: float = 12.5,
):
    width_m = 1.0
    wsd_kN_m, gk_kN_m, qk_kN_m = uls_uniform_load_per_meter(
        span_m=span_m,
        width_m=width_m,
        slab_thk_m=slab_thk_m,
        wearing_thk_m=wearing_thk_m,
        live_kN_m2=live_kN_m2,
        phi_impact=phi_impact,
    )
    wsd_kN_mN = wsd_kN_m * 1e3
    Msd_kN_m = wsd_kN_m * (span_m**2) / 8.0

    d_m = slab_thk_m - cover_m - (bar_diam_mm / 1000.0) / 2.0
    z_m = 0.9 * d_m

    fyd_mpa = steel_design_strength(fyk_mpa)
    As_req_mm2_per_m = (Msd_kN_m * 1000.0) / (fyd_mpa * z_m)

    rho_min = rho_min_slab(fck_mpa, fyk_mpa)
    As_min_mm2_per_m = rho_min * d_m * 1_000_000.0

    As_prov_mm2_per_m = max(As_req_mm2_per_m, As_min_mm2_per_m)

    bar_area_mm2 = 3.14159 * (bar_diam_mm**2) / 4.0
    spacing_mm = max(50.0, min(200.0, (bar_area_mm2 * 1000.0) / As_prov_mm2_per_m))

    Vsd_kN = wsd_kN_m * span_m / 2.0

    return {
        "w_sd_kN_per_m": wsd_kN_m,
        "gk_kN_per_m": gk_kN_m,
        "qk_kN_per_m": qk_kN_m,
        "Msd_kN_m": Msd_kN_m,
        "Vsd_kN": Vsd_kN,
        "d_m": d_m,
        "As_req_mm2_per_m": As_req_mm2_per_m,
        "As_min_mm2_per_m": As_min_mm2_per_m,
        "As_prov_mm2_per_m": As_prov_mm2_per_m,
        "bar_diam_mm": bar_diam_mm,
        "bar_spacing_mm": spacing_mm,
    }


# -----------------------------
# Viga T simplificada (viga + laje)
# -----------------------------

def design_girder(
    span_m: float,
    girder_spacing_m: float,
    num_girders: int,
    slab_thk_m: float,
    fck_mpa: float,
    fyk_mpa: float,
    live_kN_m2: float,
    wearing_thk_m: float = 0.08,
    phi_impact: float = 1.30,
    web_width_m: float = 0.30,
    girder_depth_m: float = 1.80,
    cover_m: float = 0.04,
    bar_diam_mm: float = 25.0,
):
    tributary_width_m = girder_spacing_m
    wsd_kN_m, gk_kN_m, qk_kN_m = uls_uniform_load_per_meter(
        span_m=span_m,
        width_m=tributary_width_m,
        slab_thk_m=slab_thk_m,
        wearing_thk_m=wearing_thk_m,
        live_kN_m2=live_kN_m2,
        phi_impact=phi_impact,
    )
    wsd_kN_mN = wsd_kN_m * 1e3

    Msd_kN_m = wsd_kN_m * (span_m**2) / 8.0
    Vsd_kN = wsd_kN_m * span_m / 2.0

    flange_width_m = tributary_width_m
    flange_thk_m = slab_thk_m
    web_width = web_width_m
    depth = girder_depth_m

    d_m = depth - cover_m - (bar_diam_mm / 1000.0) / 2.0
    z_m = 0.9 * d_m

    fyd_mpa = steel_design_strength(fyk_mpa)
    As_req_mm2 = (Msd_kN_m * 1000.0) / (fyd_mpa * z_m)

    As_min_mm2 = 0.0015 * web_width * d_m * 1e6
    As_prov_mm2 = max(As_req_mm2, As_min_mm2)

    bar_area_mm2 = 3.14159 * (bar_diam_mm**2) / 4.0
    num_bars = max(2, int(round(As_prov_mm2 / bar_area_mm2 + 0.499)))

    return {
        "tributary_width_m": tributary_width_m,
        "w_sd_kN_per_m": wsd_kN_m,
        "gk_kN_per_m": gk_kN_m,
        "qk_kN_per_m": qk_kN_m,
        "Msd_kN_m": Msd_kN_m,
        "Vsd_kN": Vsd_kN,
        "effective_depth_m": d_m,
        "As_req_mm2": As_req_mm2,
        "As_min_mm2": As_min_mm2,
        "As_prov_mm2": As_prov_mm2,
        "bar_diam_mm": bar_diam_mm,
        "num_bars": num_bars,
    }


def format_kN(value: float) -> str:
    return f"{value:,.2f}".replace(",", "_").replace(".", ",").replace("_", ".")


def cmd_slab(args: argparse.Namespace):
    res = design_slab(
        span_m=args.span,
        fck_mpa=args.fck,
        fyk_mpa=args.fyk,
        live_kN_m2=args.live_kN_m2,
        slab_thk_m=args.slab_thk_m,
        wearing_thk_m=args.pav_thk_m,
        phi_impact=args.phi,
        cover_m=args.cover_m,
        bar_diam_mm=args.bar_diam_mm,
    )
    print("== Laje (faixa 1,00 m) ==")
    print(f"L = {args.span:.2f} m | h = {args.slab_thk_m*100:.0f} cm | fck = {args.fck:.0f} MPa | fyk = {args.fyk:.0f} MPa")
    print(f"w_sd = {format_kN(res['w_sd_kN_per_m'])} kN/m (gk={format_kN(res['gk_kN_per_m'])}, qk={format_kN(res['qk_kN_per_m'])})")
    print(f"Msd = {format_kN(res['Msd_kN_m'])} kN.m | Vsd = {format_kN(res['Vsd_kN'])} kN")
    print(f"d ≈ {res['d_m']*100:.1f} cm | z ≈ 0.9 d")
    print(f"As,req = {res['As_req_mm2_per_m']:.0f} mm²/m | As,min = {res['As_min_mm2_per_m']:.0f} mm²/m")
    print(f"Adotar: Ø{res['bar_diam_mm']:.0f} c/{res['bar_spacing_mm']:.0f} mm")


def cmd_girder(args: argparse.Namespace):
    res = design_girder(
        span_m=args.span,
        girder_spacing_m=args.spacing,
        num_girders=args.num_girders,
        slab_thk_m=args.slab_thk_m,
        fck_mpa=args.fck,
        fyk_mpa=args.fyk,
        live_kN_m2=args.live_kN_m2,
        wearing_thk_m=args.pav_thk_m,
        phi_impact=args.phi,
        web_width_m=args.web_width_m,
        girder_depth_m=args.girder_depth_m,
        cover_m=args.cover_m,
        bar_diam_mm=args.bar_diam_mm,
    )
    print("== Viga + Laje (Viga T simplificada) ==")
    print(f"L = {args.span:.2f} m | espaçamento = {args.spacing:.2f} m | N vigas = {args.num_girders}")
    print(f"fck = {args.fck:.0f} MPa | fyk = {args.fyk:.0f} MPa | h_viga = {args.girder_depth_m:.2f} m")
    print(f"w_sd = {format_kN(res['w_sd_kN_per_m'])} kN/m (gk={format_kN(res['gk_kN_per_m'])}, qk={format_kN(res['qk_kN_per_m'])})")
    print(f"Msd = {format_kN(res['Msd_kN_m'])} kN.m | Vsd = {format_kN(res['Vsd_kN'])} kN")
    print(f"d ≈ {res['effective_depth_m']*100:.1f} cm | z ≈ 0.9 d")
    print(f"As,req = {res['As_req_mm2']:.0f} mm² | As,min = {res['As_min_mm2']:.0f} mm² | Adotar: Ø{res['bar_diam_mm']:.0f} - {res['num_bars']} barras")


def build_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(description="Pré-dimensionamento de pontes de CA (simplificado)")
    sub = parser.add_subparsers(dest="cmd", required=True)

    p_slab = sub.add_parser("slab", help="Pré-dimensionar laje (faixa 1,00 m)")
    p_slab.add_argument("--span", type=float, required=True, help="Vão (m)")
    p_slab.add_argument("--fck", type=float, default=30.0)
    p_slab.add_argument("--fyk", type=float, default=500.0)
    p_slab.add_argument("--live_kN_m2", type=float, default=5.0, help="Sobrecarga equivalente (kN/m²)")
    p_slab.add_argument("--slab_thk_m", type=float, default=0.20, help="Espessura da laje (m)")
    p_slab.add_argument("--pav_thk_m", type=float, default=0.08, help="Espessura revestimento/asfalto (m)")
    p_slab.add_argument("--phi", type=float, default=1.30, help="Coef. impacto/amplificação")
    p_slab.add_argument("--cover_m", type=float, default=0.035, help="Cobrimento (m)")
    p_slab.add_argument("--bar_diam_mm", type=float, default=12.5)
    p_slab.set_defaults(func=cmd_slab)

    p_g = sub.add_parser("girder", help="Pré-dimensionar viga + laje (Viga T)")
    p_g.add_argument("--span", type=float, required=True, help="Vão (m)")
    p_g.add_argument("--spacing", type=float, required=True, help="Espaçamento entre vigas (m)")
    p_g.add_argument("--num_girders", type=int, default=4)
    p_g.add_argument("--slab_thk_m", type=float, default=0.20)
    p_g.add_argument("--fck", type=float, default=35.0)
    p_g.add_argument("--fyk", type=float, default=500.0)
    p_g.add_argument("--live_kN_m2", type=float, default=5.0)
    p_g.add_argument("--pav_thk_m", type=float, default=0.08)
    p_g.add_argument("--phi", type=float, default=1.30)
    p_g.add_argument("--web_width_m", type=float, default=0.30)
    p_g.add_argument("--girder_depth_m", type=float, default=1.80)
    p_g.add_argument("--cover_m", type=float, default=0.040)
    p_g.add_argument("--bar_diam_mm", type=float, default=25.0)
    p_g.set_defaults(func=cmd_girder)

    return parser


def main():
    parser = build_parser()
    args = parser.parse_args()
    args.func(args)


if __name__ == "__main__":
    main()

