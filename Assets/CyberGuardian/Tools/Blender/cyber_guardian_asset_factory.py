"""
Cyber Guardian procedural asset factory for Blender.

Usage:
  blender --background --python "C:/Users/Wafda/My project/Assets/CyberGuardian/Tools/Blender/cyber_guardian_asset_factory.py" -- --out "C:/Users/Wafda/My project/Assets/CyberGuardian/GeneratedBlenderAssets"

The script creates modular cyber-themed source assets and exports GLB files:
  - cg_guardian_player.glb: playable Cyber Guardian with idle/run/jump/melee frame ranges
  - cg_virus_grunt.glb: patrol virus enemy with idle/patrol/attack frame ranges
  - cg_malware_boss.glb: boss core with pulse/attack/enraged frame ranges
  - cg_quiz_shield_blocks.glb: four quiz shield block variants with idle/wrong/clear ranges
  - cg_platform_traps.glb: platform tiles, spikes, packet laser, data pipe, jump pad
  - cg_projectiles.glb: player patch core, boss packet shot, melee slash visual

Recommended Unity clip splits after import:
  Guardian: Idle 1-48, Run 60-96, Jump 110-130, Melee 140-170
  Virus: Idle 1-48, Patrol 60-100, Attack 120-150
  Boss: Pulse 1-60, Attack 80-120, Enraged 140-180
  Shield Blocks: Idle 1-48, Wrong 60-85, Clear 100-130
  Projectiles/Traps: Loop/Pulse 1-60
"""

from __future__ import annotations

import argparse
import math
import sys
from pathlib import Path

import bpy
from mathutils import Euler, Vector


DEFAULT_OUT = Path(r"C:/Users/Wafda/My project/Assets/CyberGuardian/GeneratedBlenderAssets")


def parse_args() -> argparse.Namespace:
    args = sys.argv
    user_args = args[args.index("--") + 1 :] if "--" in args else []
    parser = argparse.ArgumentParser(description="Build Cyber Guardian Blender assets.")
    parser.add_argument("--out", default=str(DEFAULT_OUT), help="Output directory for blend/glb assets.")
    parser.add_argument("--no-glb", action="store_true", help="Only save the .blend file.")
    parser.add_argument("--no-render", action="store_true", help="Skip transparent PNG sprite renders.")
    return parser.parse_args(user_args)


def clear_scene() -> None:
    bpy.ops.object.select_all(action="SELECT")
    bpy.ops.object.delete()
    for block in (bpy.data.meshes, bpy.data.materials, bpy.data.images, bpy.data.curves):
        for item in list(block):
            if item.users == 0:
                block.remove(item)


def make_collection(name: str) -> bpy.types.Collection:
    collection = bpy.data.collections.new(name)
    bpy.context.scene.collection.children.link(collection)
    return collection


def link_to_collection(obj: bpy.types.Object, collection: bpy.types.Collection) -> bpy.types.Object:
    for current in list(obj.users_collection):
        current.objects.unlink(obj)
    collection.objects.link(obj)
    return obj


def make_mat(name: str, color: tuple[float, float, float, float], emission: float = 0.0) -> bpy.types.Material:
    mat = bpy.data.materials.new(name)
    mat.use_nodes = True
    nodes = mat.node_tree.nodes
    principled = nodes.get("Principled BSDF")
    if principled is not None:
        principled.inputs["Base Color"].default_value = color
        principled.inputs["Roughness"].default_value = 0.58
        principled.inputs["Metallic"].default_value = 0.08
        if "Emission Color" in principled.inputs:
            principled.inputs["Emission Color"].default_value = color
        if "Emission Strength" in principled.inputs:
            principled.inputs["Emission Strength"].default_value = emission
    mat.diffuse_color = color
    return mat


def assign_mat(obj: bpy.types.Object, mat: bpy.types.Material) -> bpy.types.Object:
    if hasattr(obj.data, "materials"):
        obj.data.materials.append(mat)
    return obj


def bevel(obj: bpy.types.Object, amount: float, segments: int = 2) -> bpy.types.Object:
    modifier = obj.modifiers.new(name="Soft bevel", type="BEVEL")
    modifier.width = amount
    modifier.segments = segments
    modifier.affect = "EDGES"
    obj.modifiers.new(name="Weighted normals", type="WEIGHTED_NORMAL")
    return obj


def cube(
    name: str,
    collection: bpy.types.Collection,
    loc: tuple[float, float, float],
    scale: tuple[float, float, float],
    mat: bpy.types.Material,
    parent: bpy.types.Object | None = None,
    bevel_amount: float = 0.0,
) -> bpy.types.Object:
    bpy.ops.mesh.primitive_cube_add(size=1.0, location=loc)
    obj = bpy.context.object
    obj.name = name
    obj.scale = scale
    assign_mat(obj, mat)
    link_to_collection(obj, collection)
    if parent is not None:
        obj.parent = parent
    if bevel_amount > 0.0:
        bevel(obj, bevel_amount)
    return obj


def sphere(
    name: str,
    collection: bpy.types.Collection,
    loc: tuple[float, float, float],
    scale: tuple[float, float, float],
    mat: bpy.types.Material,
    parent: bpy.types.Object | None = None,
    segments: int = 32,
) -> bpy.types.Object:
    bpy.ops.mesh.primitive_uv_sphere_add(segments=segments, ring_count=16, radius=1.0, location=loc)
    obj = bpy.context.object
    obj.name = name
    obj.scale = scale
    assign_mat(obj, mat)
    link_to_collection(obj, collection)
    if parent is not None:
        obj.parent = parent
    obj.modifiers.new(name="Weighted normals", type="WEIGHTED_NORMAL")
    return obj


def cylinder(
    name: str,
    collection: bpy.types.Collection,
    loc: tuple[float, float, float],
    radius: float,
    depth: float,
    mat: bpy.types.Material,
    parent: bpy.types.Object | None = None,
    vertices: int = 24,
    rot: tuple[float, float, float] = (0.0, 0.0, 0.0),
) -> bpy.types.Object:
    bpy.ops.mesh.primitive_cylinder_add(vertices=vertices, radius=radius, depth=depth, location=loc, rotation=rot)
    obj = bpy.context.object
    obj.name = name
    assign_mat(obj, mat)
    link_to_collection(obj, collection)
    if parent is not None:
        obj.parent = parent
    bevel(obj, radius * 0.08, 1)
    return obj


def cone(
    name: str,
    collection: bpy.types.Collection,
    loc: tuple[float, float, float],
    radius: float,
    depth: float,
    mat: bpy.types.Material,
    parent: bpy.types.Object | None = None,
    vertices: int = 4,
    rot: tuple[float, float, float] = (0.0, 0.0, 0.0),
) -> bpy.types.Object:
    bpy.ops.mesh.primitive_cone_add(vertices=vertices, radius1=radius, radius2=0.0, depth=depth, location=loc, rotation=rot)
    obj = bpy.context.object
    obj.name = name
    assign_mat(obj, mat)
    link_to_collection(obj, collection)
    if parent is not None:
        obj.parent = parent
    return obj


def torus(
    name: str,
    collection: bpy.types.Collection,
    loc: tuple[float, float, float],
    major: float,
    minor: float,
    mat: bpy.types.Material,
    parent: bpy.types.Object | None = None,
    rot: tuple[float, float, float] = (0.0, 0.0, 0.0),
) -> bpy.types.Object:
    bpy.ops.mesh.primitive_torus_add(major_radius=major, minor_radius=minor, major_segments=64, minor_segments=8, location=loc, rotation=rot)
    obj = bpy.context.object
    obj.name = name
    assign_mat(obj, mat)
    link_to_collection(obj, collection)
    if parent is not None:
        obj.parent = parent
    return obj


def text_mesh(
    name: str,
    collection: bpy.types.Collection,
    text: str,
    loc: tuple[float, float, float],
    size: float,
    mat: bpy.types.Material,
    parent: bpy.types.Object | None = None,
    rot: tuple[float, float, float] = (math.radians(90), 0.0, 0.0),
) -> bpy.types.Object:
    bpy.ops.object.text_add(location=loc, rotation=rot)
    obj = bpy.context.object
    obj.name = name
    obj.data.body = text
    obj.data.align_x = "CENTER"
    obj.data.align_y = "CENTER"
    obj.data.size = size
    obj.data.extrude = 0.01
    assign_mat(obj, mat)
    link_to_collection(obj, collection)
    if parent is not None:
        obj.parent = parent
    return obj


def empty(name: str, collection: bpy.types.Collection, loc: tuple[float, float, float] = (0.0, 0.0, 0.0)) -> bpy.types.Object:
    obj = bpy.data.objects.new(name, None)
    obj.empty_display_type = "PLAIN_AXES"
    obj.empty_display_size = 0.35
    obj.location = loc
    collection.objects.link(obj)
    return obj


def key(obj: bpy.types.Object, frame: int, loc=None, rot=None, scale=None) -> None:
    bpy.context.scene.frame_set(frame)
    if loc is not None:
        obj.location = loc
        obj.keyframe_insert(data_path="location", frame=frame)
    if rot is not None:
        obj.rotation_euler = Euler(rot, "XYZ")
        obj.keyframe_insert(data_path="rotation_euler", frame=frame)
    if scale is not None:
        obj.scale = scale
        obj.keyframe_insert(data_path="scale", frame=frame)


def add_marker(name: str, frame: int) -> None:
    bpy.context.scene.timeline_markers.new(name, frame=frame)


def descendants(root: bpy.types.Object) -> list[bpy.types.Object]:
    found = [root]
    for child in root.children:
        found.extend(descendants(child))
    return found


def select_hierarchy(root: bpy.types.Object) -> None:
    bpy.ops.object.select_all(action="DESELECT")
    for obj in descendants(root):
        obj.select_set(True)
    bpy.context.view_layer.objects.active = root


def export_root(root: bpy.types.Object, out_dir: Path, filename: str) -> None:
    select_hierarchy(root)
    bpy.ops.export_scene.gltf(
        filepath=str(out_dir / filename),
        use_selection=True,
        export_format="GLB",
        export_animations=True,
        export_frame_range=True,
        export_apply=True,
    )


def evaluated_bounds(objects: list[bpy.types.Object]) -> tuple[Vector, Vector]:
    depsgraph = bpy.context.evaluated_depsgraph_get()
    min_v = Vector((float("inf"), float("inf"), float("inf")))
    max_v = Vector((float("-inf"), float("-inf"), float("-inf")))
    has_bounds = False

    for obj in objects:
        if obj.type not in {"MESH", "CURVE", "FONT"}:
            continue

        evaluated = obj.evaluated_get(depsgraph)
        for corner in evaluated.bound_box:
            world = evaluated.matrix_world @ Vector(corner)
            min_v.x = min(min_v.x, world.x)
            min_v.y = min(min_v.y, world.y)
            min_v.z = min(min_v.z, world.z)
            max_v.x = max(max_v.x, world.x)
            max_v.y = max(max_v.y, world.y)
            max_v.z = max(max_v.z, world.z)
            has_bounds = True

    if not has_bounds:
        return Vector((-1.0, -1.0, 0.0)), Vector((1.0, 1.0, 2.0))

    return min_v, max_v


def set_root_render_visibility(roots: list[bpy.types.Object], active_root: bpy.types.Object) -> None:
    visible = set(descendants(active_root))
    for obj in bpy.context.scene.objects:
        if obj.type in {"CAMERA", "LIGHT"}:
            obj.hide_render = False
            obj.hide_viewport = False
        else:
            obj.hide_render = obj not in visible
            obj.hide_viewport = obj not in visible


def render_root_sprite(root: bpy.types.Object, roots: list[bpy.types.Object], out_dir: Path, filename: str, frame: int = 1) -> None:
    set_root_render_visibility(roots, root)
    bpy.context.scene.frame_set(frame)
    objects = descendants(root)
    min_v, max_v = evaluated_bounds(objects)
    center = (min_v + max_v) * 0.5
    width = max(max_v.x - min_v.x, 0.25)
    height = max(max_v.z - min_v.z, 0.25)

    camera = bpy.context.scene.camera
    camera.location = (center.x, min_v.y - 7.5, center.z)
    camera.rotation_euler = (math.radians(90), 0.0, 0.0)
    camera.data.type = "ORTHO"
    camera.data.ortho_scale = max(width, height) * 1.25

    bpy.context.scene.render.resolution_x = 512
    bpy.context.scene.render.resolution_y = 512
    bpy.context.scene.render.film_transparent = True
    bpy.context.scene.render.image_settings.file_format = "PNG"
    bpy.context.scene.render.filepath = str(out_dir / filename)
    bpy.ops.render.render(write_still=True)


def render_sprite_set(roots: list[tuple[bpy.types.Object, str]], out_dir: Path) -> None:
    sprite_specs = [
        ("cg_guardian_player_sprite.png", 24),
        ("cg_virus_grunt_sprite.png", 24),
        ("cg_malware_boss_sprite.png", 30),
        ("cg_quiz_shield_blocks_sprite.png", 1),
        ("cg_platform_traps_sprite.png", 30),
        ("cg_projectiles_sprite.png", 18),
    ]
    root_only = [root for root, _ in roots]
    for (root, _), (filename, frame) in zip(roots, sprite_specs):
        render_root_sprite(root, root_only, out_dir, filename, frame)

    patch_core = bpy.data.objects.get("patch_core_projectile")
    if patch_core is not None:
        render_root_sprite(patch_core, [patch_core], out_dir, "cg_patch_core_sprite.png", 30)

    for obj in bpy.context.scene.objects:
        obj.hide_render = False
        obj.hide_viewport = False


def create_materials() -> dict[str, bpy.types.Material]:
    return {
        "guardian": make_mat("guardian_black_cyber_armor", (0.015, 0.07, 0.10, 1.0), 0.08),
        "guardian_dark": make_mat("guardian_graphite_joints", (0.02, 0.025, 0.035, 1.0), 0.0),
        "guardian_white": make_mat("guardian_white_panels", (0.82, 0.95, 1.0, 1.0), 0.05),
        "guardian_cape": make_mat("guardian_data_cape", (0.0, 0.025, 0.035, 1.0), 0.18),
        "cyan_glow": make_mat("cyan_security_glow", (0.0, 0.92, 1.0, 1.0), 1.5),
        "green_glow": make_mat("green_verified_glow", (0.25, 1.0, 0.24, 1.0), 1.2),
        "virus": make_mat("virus_magenta_membrane", (0.92, 0.04, 0.36, 1.0), 0.65),
        "virus_dark": make_mat("virus_black_armor", (0.04, 0.005, 0.025, 1.0), 0.12),
        "boss": make_mat("boss_malware_core", (0.95, 0.05, 0.12, 1.0), 0.85),
        "boss_dark": make_mat("boss_black_shell", (0.08, 0.02, 0.03, 1.0), 0.05),
        "password": make_mat("quiz_password_blue", (0.12, 0.48, 1.0, 1.0), 0.45),
        "malware": make_mat("quiz_malware_green", (0.24, 0.86, 0.18, 1.0), 0.45),
        "network": make_mat("quiz_network_yellow", (1.0, 0.76, 0.12, 1.0), 0.35),
        "privacy": make_mat("quiz_privacy_violet", (0.72, 0.28, 1.0, 1.0), 0.45),
        "text": make_mat("label_white", (0.92, 0.96, 1.0, 1.0), 0.35),
        "platform": make_mat("platform_circuit_plate", (0.13, 0.31, 0.35, 1.0), 0.05),
        "platform_edge": make_mat("platform_edge_gold", (1.0, 0.63, 0.14, 1.0), 0.15),
        "trap": make_mat("trap_warning_red", (1.0, 0.12, 0.08, 1.0), 0.75),
        "projectile": make_mat("patch_core_projectile", (0.35, 0.95, 1.0, 1.0), 1.2),
        "boss_packet": make_mat("boss_packet_orange", (1.0, 0.38, 0.06, 1.0), 0.9),
    }


def build_guardian(mats: dict[str, bpy.types.Material]) -> bpy.types.Object:
    collection = make_collection("CG_Guardian_Player")
    root = empty("CG_Guardian_Player", collection)

    cape = cube("guardian_fragmented_data_cape", collection, (-0.22, 0.08, 1.34), (0.34, 0.045, 0.86), mats["guardian_cape"], root, 0.025)
    body = cube("guardian_body_armor", collection, (0, 0, 1.55), (0.42, 0.2, 0.58), mats["guardian"], root, 0.05)
    chest_plate = cube("guardian_white_chest_plate", collection, (0.0, -0.22, 1.72), (0.28, 0.035, 0.18), mats["guardian_white"], root, 0.02)
    core = sphere("guardian_security_core", collection, (0, -0.215, 1.62), (0.18, 0.04, 0.18), mats["cyan_glow"], root, 32)
    head = cube("guardian_helmet", collection, (0, 0, 2.28), (0.36, 0.2, 0.28), mats["guardian_white"], root, 0.04)
    visor = cube("guardian_visor", collection, (0, -0.215, 2.3), (0.24, 0.025, 0.055), mats["cyan_glow"], root, 0.015)
    halo = torus("guardian_shield_halo", collection, (0, 0, 1.58), 0.64, 0.018, mats["cyan_glow"], root, (math.radians(90), 0, 0))
    left_shoulder = cube("guardian_left_shoulder_plate", collection, (-0.46, -0.02, 1.92), (0.18, 0.16, 0.12), mats["cyan_glow"], root, 0.025)
    right_shoulder = cube("guardian_right_shoulder_plate", collection, (0.46, -0.02, 1.92), (0.18, 0.16, 0.12), mats["guardian_white"], root, 0.025)

    left_arm = cylinder("guardian_left_arm", collection, (-0.58, 0, 1.68), 0.07, 0.58, mats["guardian_dark"], root, 16, (0, math.radians(90), 0))
    right_arm = cylinder("guardian_right_arm", collection, (0.58, 0, 1.68), 0.07, 0.58, mats["guardian_dark"], root, 16, (0, math.radians(90), 0))
    blade = cube("guardian_melee_blade", collection, (1.06, -0.02, 1.65), (0.54, 0.028, 0.065), mats["cyan_glow"], root, 0.02)
    left_leg = cylinder("guardian_left_leg", collection, (-0.18, 0, 0.78), 0.08, 0.66, mats["guardian_dark"], root, 16)
    right_leg = cylinder("guardian_right_leg", collection, (0.18, 0, 0.78), 0.08, 0.66, mats["guardian_dark"], root, 16)
    left_boot = cube("guardian_left_boot", collection, (-0.19, -0.02, 0.38), (0.16, 0.23, 0.08), mats["guardian"], root, 0.025)
    right_boot = cube("guardian_right_boot", collection, (0.19, -0.02, 0.38), (0.16, 0.23, 0.08), mats["guardian"], root, 0.025)

    parts = [root, cape, body, chest_plate, core, head, visor, halo, left_shoulder, right_shoulder, left_arm, right_arm, blade, left_leg, right_leg, left_boot, right_boot]
    for obj in parts:
        obj["asset_role"] = "playable_guardian"

    for frame, z, pulse in [(1, 0.0, 1.0), (24, 0.045, 1.07), (48, 0.0, 1.0)]:
        key(root, frame, loc=(0, 0, z))
        key(halo, frame, scale=(pulse, pulse, pulse))
        key(core, frame, scale=(0.18 * pulse, 0.04, 0.18 * pulse))

    for frame, left_rot, right_rot, x in [(60, -0.45, 0.45, 0), (72, 0.45, -0.45, 0.04), (84, -0.45, 0.45, 0), (96, 0.45, -0.45, 0.04)]:
        key(left_leg, frame, rot=(left_rot, 0, 0))
        key(right_leg, frame, rot=(right_rot, 0, 0))
        key(left_arm, frame, rot=(0, math.radians(90), left_rot))
        key(right_arm, frame, rot=(0, math.radians(90), right_rot))
        key(root, frame, loc=(x, 0, 0.0))

    key(root, 110, loc=(0, 0, 0.0))
    key(root, 120, loc=(0.05, 0, 0.34), scale=(0.95, 0.95, 1.05))
    key(root, 130, loc=(0, 0, 0.0), scale=(1.0, 1.0, 1.0))

    key(right_arm, 140, rot=(0, math.radians(90), -0.15))
    key(blade, 140, loc=(0.98, -0.02, 1.65), scale=(0.36, 0.035, 0.075))
    key(right_arm, 154, rot=(0, math.radians(90), -1.25))
    key(blade, 154, loc=(1.16, -0.02, 1.72), scale=(0.6, 0.04, 0.1))
    key(right_arm, 170, rot=(0, math.radians(90), -0.15))
    key(blade, 170, loc=(0.98, -0.02, 1.65), scale=(0.36, 0.035, 0.075))

    return root


def build_virus(mats: dict[str, bpy.types.Material]) -> bpy.types.Object:
    collection = make_collection("CG_Virus_Grunt")
    root = empty("CG_Virus_Grunt", collection)
    torso = cube("virus_hunched_torso", collection, (0, 0, 1.04), (0.38, 0.22, 0.46), mats["virus_dark"], root, 0.045)
    body = sphere("virus_corrupt_core", collection, (0, -0.24, 1.12), (0.20, 0.055, 0.20), mats["virus"], root, 32)
    head = cube("virus_skull_helmet", collection, (0.05, 0, 1.65), (0.28, 0.18, 0.22), mats["virus_dark"], root, 0.035)
    eye = sphere("virus_single_eye", collection, (0.1, -0.2, 1.68), (0.13, 0.025, 0.13), mats["virus"], root, 24)
    pupil = sphere("virus_eye_pupil", collection, (0.1, -0.225, 1.68), (0.045, 0.012, 0.045), mats["text"], root, 16)
    left_arm = cylinder("virus_left_clawed_arm", collection, (-0.42, 0, 1.12), 0.055, 0.55, mats["virus_dark"], root, 12, (0, math.radians(72), math.radians(-18)))
    right_arm = cylinder("virus_right_clawed_arm", collection, (0.48, 0, 1.1), 0.055, 0.66, mats["virus_dark"], root, 12, (0, math.radians(105), math.radians(18)))
    left_claw = cone("virus_left_packet_claw", collection, (-0.72, -0.02, 1.02), 0.07, 0.22, mats["virus"], root, 4, (0, math.radians(90), 0))
    right_claw = cone("virus_right_packet_claw", collection, (0.82, -0.02, 1.08), 0.08, 0.26, mats["virus"], root, 4, (0, math.radians(-90), 0))

    for i in range(10):
        angle = i * math.tau / 10.0
        loc = (math.cos(angle) * 0.44, math.sin(angle) * 0.22, 1.0 + math.sin(angle * 2.0) * 0.22)
        spike = cone(f"virus_spike_{i:02d}", collection, loc, 0.08, 0.25, mats["virus_dark"], root, 4, (0, math.radians(90), angle))
        spike["asset_role"] = "enemy_spike"

    for frame, y, sx in [(1, 0.0, 1.0), (24, -0.02, 1.08), (48, 0.0, 1.0)]:
        key(body, frame, scale=(0.20 * sx, 0.055 * sx, 0.20 / sx))
        key(torso, frame, scale=(0.38 * sx, 0.22, 0.46 / sx))
        key(root, frame, loc=(0, 0, y))
    for frame, x, rot in [(60, -0.28, -0.18), (80, 0.28, 0.18), (100, -0.28, -0.18)]:
        key(root, frame, loc=(x, 0, 0), rot=(0, 0, rot))
    for frame, scale in [(120, 1.0), (135, 1.35), (150, 1.0)]:
        key(eye, frame, scale=(0.13 * scale, 0.025, 0.13 * scale))
        key(pupil, frame, scale=(0.045 * scale, 0.012, 0.045 * scale))

    for obj in descendants(root):
        obj["asset_role"] = "virus_grunt"
    return root


def build_boss(mats: dict[str, bpy.types.Material]) -> bpy.types.Object:
    collection = make_collection("CG_Malware_Boss")
    root = empty("CG_Malware_Boss", collection)
    shell = sphere("boss_black_shell", collection, (0, 0, 1.7), (0.95, 0.3, 0.95), mats["boss_dark"], root, 48)
    core = sphere("boss_red_core", collection, (0, -0.32, 1.7), (0.54, 0.08, 0.54), mats["boss"], root, 48)
    ring_a = torus("boss_outer_firewall_ring", collection, (0, -0.01, 1.7), 1.12, 0.025, mats["boss"], root, (math.radians(90), 0, 0))
    ring_b = torus("boss_inner_firewall_ring", collection, (0, -0.02, 1.7), 0.72, 0.02, mats["boss_packet"], root, (math.radians(90), 0, 0))

    for i in range(8):
        angle = i * math.tau / 8.0
        tentacle = cylinder(
            f"boss_data_tentacle_{i:02d}",
            collection,
            (math.cos(angle) * 0.9, math.sin(angle) * 0.08, 1.7 + math.sin(angle) * 0.72),
            0.035,
            0.92,
            mats["boss_dark"],
            root,
            12,
            (math.radians(90), 0, angle),
        )
        tip = sphere(
            f"boss_tentacle_tip_{i:02d}",
            collection,
            (math.cos(angle) * 1.32, math.sin(angle) * 0.12, 1.7 + math.sin(angle) * 1.05),
            (0.09, 0.04, 0.09),
            mats["boss"],
            root,
            16,
        )
        tentacle["asset_role"] = "boss_tentacle"
        tip["asset_role"] = "boss_weapon_tip"

    for frame, scale, rot in [(1, 1.0, 0.0), (30, 1.08, math.radians(8)), (60, 1.0, 0.0)]:
        key(core, frame, scale=(0.54 * scale, 0.08, 0.54 * scale))
        key(ring_a, frame, rot=(math.radians(90), 0, rot))
        key(ring_b, frame, rot=(math.radians(90), 0, -rot * 1.4))

    for frame, x, glow in [(80, 0.0, 1.0), (100, -0.2, 1.28), (120, 0.0, 1.0)]:
        key(root, frame, loc=(x, 0, 0))
        key(core, frame, scale=(0.54 * glow, 0.08, 0.54 * glow))

    for frame, scale, rot in [(140, 1.08, 0.0), (160, 1.2, math.radians(16)), (180, 1.08, 0.0)]:
        key(shell, frame, scale=(0.95 * scale, 0.3, 0.95 * scale))
        key(ring_a, frame, rot=(math.radians(90), 0, rot))
        key(ring_b, frame, rot=(math.radians(90), 0, -rot))

    for obj in descendants(root):
        obj["asset_role"] = "malware_boss"
    return root


def build_quiz_blocks(mats: dict[str, bpy.types.Material]) -> bpy.types.Object:
    collection = make_collection("CG_Quiz_Shield_Blocks")
    root = empty("CG_Quiz_Shield_Blocks", collection)
    categories = [
        ("Password", "PW", mats["password"], -1.8),
        ("Malware", "MW", mats["malware"], -0.6),
        ("Network", "NT", mats["network"], 0.6),
        ("Privacy", "PR", mats["privacy"], 1.8),
    ]

    for name, label, mat, x in categories:
        block_root = empty(f"quiz_{name.lower()}_block", collection, (x, 0, 0))
        block_root.parent = root
        panel = cube(f"{name}_shield_panel", collection, (x, 0, 1.0), (0.44, 0.12, 0.34), mat, block_root, 0.045)
        rim = torus(f"{name}_shield_rim", collection, (x, -0.13, 1.0), 0.47, 0.014, mats["text"], block_root, (math.radians(90), 0, 0))
        label_obj = text_mesh(f"{name}_quiz_label", collection, label, (x, -0.145, 1.01), 0.18, mats["text"], block_root)
        panel["quiz_category"] = name
        label_obj["quiz_category"] = name

        for frame, s in [(1, 1.0), (24, 1.04), (48, 1.0)]:
            key(block_root, frame, scale=(s, s, s))
        for frame, dx in [(60, 0.0), (66, 0.08), (72, -0.08), (78, 0.04), (85, 0.0)]:
            key(block_root, frame, loc=(x + dx, 0, 0))
        for frame, s in [(100, 1.0), (115, 0.65), (130, 0.02)]:
            key(block_root, frame, scale=(s, s, s))
            key(rim, frame, scale=(s, s, s))

    for obj in descendants(root):
        obj["asset_role"] = "quiz_shield_block"
    return root


def build_platform_traps(mats: dict[str, bpy.types.Material]) -> bpy.types.Object:
    collection = make_collection("CG_Platform_Traps")
    root = empty("CG_Platform_Traps", collection)

    base = cube("circuit_ground_tile", collection, (-2.2, 0, 0.3), (0.8, 0.2, 0.16), mats["platform"], root, 0.025)
    edge = cube("circuit_ground_gold_edge", collection, (-2.2, -0.01, 0.49), (0.82, 0.22, 0.035), mats["platform_edge"], root, 0.012)
    for i in range(3):
        cube(f"circuit_trace_{i}", collection, (-2.45 + i * 0.25, -0.22, 0.52), (0.075, 0.015, 0.01), mats["cyan_glow"], root, 0.004)

    jump_pad = cube("jump_pad_security_booster", collection, (-0.7, 0, 0.28), (0.42, 0.18, 0.08), mats["green_glow"], root, 0.03)
    pad_ring = torus("jump_pad_energy_ring", collection, (-0.7, -0.03, 0.38), 0.36, 0.012, mats["green_glow"], root, (math.radians(90), 0, 0))
    for frame, z, scale in [(1, 0.38, 1.0), (30, 0.48, 1.18), (60, 0.38, 1.0)]:
        key(pad_ring, frame, loc=(-0.7, -0.03, z), scale=(scale, scale, scale))

    spike_base = cube("spike_trap_base", collection, (0.85, 0, 0.18), (0.55, 0.18, 0.07), mats["trap"], root, 0.015)
    for i in range(5):
        cone(f"memory_leak_spike_{i}", collection, (0.45 + i * 0.2, 0, 0.42), 0.08, 0.32, mats["trap"], root, 4)

    post_a = cube("packet_laser_post_a", collection, (2.05, 0, 0.72), (0.08, 0.12, 0.52), mats["platform_edge"], root, 0.02)
    post_b = cube("packet_laser_post_b", collection, (2.75, 0, 0.72), (0.08, 0.12, 0.52), mats["platform_edge"], root, 0.02)
    beam = cube("packet_laser_beam", collection, (2.4, -0.03, 0.85), (0.38, 0.035, 0.035), mats["trap"], root, 0.012)
    for frame, sy in [(1, 1.0), (30, 1.5), (60, 1.0)]:
        key(beam, frame, scale=(0.38, 0.035 * sy, 0.035 * sy))

    pipe = cylinder("background_data_pipe", collection, (3.8, 0.08, 0.85), 0.08, 1.1, mats["platform"], root, 20, (0, math.radians(90), 0))
    pipe_glow = cylinder("background_data_pipe_glow", collection, (3.8, -0.02, 0.85), 0.035, 1.15, mats["cyan_glow"], root, 16, (0, math.radians(90), 0))
    for frame, sx in [(1, 1.0), (30, 1.25), (60, 1.0)]:
        key(pipe_glow, frame, scale=(sx, sx, sx))

    for obj in descendants(root):
        obj["asset_role"] = "platform_or_trap"
    return root


def build_projectiles(mats: dict[str, bpy.types.Material]) -> bpy.types.Object:
    collection = make_collection("CG_Projectiles")
    root = empty("CG_Projectiles", collection)

    patch_root = empty("patch_core_projectile", collection, (-1.5, 0, 1.0))
    patch_root.parent = root
    patch = sphere("patch_core_orb", collection, (-1.5, 0, 1.0), (0.16, 0.16, 0.16), mats["projectile"], patch_root, 24)
    patch_ring = torus("patch_core_spin_ring", collection, (-1.5, 0, 1.0), 0.22, 0.012, mats["cyan_glow"], patch_root, (math.radians(90), 0, 0))
    for frame, rot in [(1, 0), (30, math.pi), (60, math.tau)]:
        key(patch_ring, frame, rot=(math.radians(90), 0, rot))
        key(patch_root, frame, loc=(-1.5 + frame / 60.0 * 0.25, 0, 1.0))

    packet_root = empty("boss_packet_projectile", collection, (0.0, 0, 1.0))
    packet_root.parent = root
    packet = cube("boss_packet_core", collection, (0, 0, 1.0), (0.18, 0.11, 0.11), mats["boss_packet"], packet_root, 0.025)
    trail = cone("boss_packet_trail", collection, (0.28, 0, 1.0), 0.12, 0.38, mats["boss"], packet_root, 24, (0, math.radians(90), 0))
    for frame, sx in [(1, 1.0), (30, 1.25), (60, 1.0)]:
        key(packet, frame, scale=(0.18 * sx, 0.11, 0.11))
        key(trail, frame, scale=(sx, sx, sx))

    slash_root = empty("guardian_melee_slash", collection, (1.4, 0, 1.0))
    slash_root.parent = root
    slash = torus("guardian_slash_arc", collection, (1.4, -0.02, 1.0), 0.34, 0.016, mats["cyan_glow"], slash_root, (math.radians(90), 0, math.radians(45)))
    slash.scale = (1.0, 0.18, 1.0)
    for frame, scale, rot in [(1, 0.2, math.radians(-30)), (18, 1.1, math.radians(40)), (36, 0.05, math.radians(90))]:
        key(slash, frame, scale=(scale, 0.18 * scale, scale), rot=(math.radians(90), 0, rot))

    for obj in descendants(root):
        obj["asset_role"] = "projectile_or_melee_vfx"
    return root


def setup_scene() -> None:
    bpy.context.scene.frame_start = 1
    bpy.context.scene.frame_end = 180
    for name, frame in [
        ("Guardian_Idle_Start", 1),
        ("Guardian_Run_Start", 60),
        ("Guardian_Jump_Start", 110),
        ("Guardian_Melee_Start", 140),
        ("Virus_Attack_Start", 120),
        ("Boss_Attack_Start", 80),
        ("Shield_Wrong_Start", 60),
        ("Shield_Clear_Start", 100),
    ]:
        add_marker(name, frame)

    bpy.ops.object.light_add(type="AREA", location=(0, -4, 6))
    light = bpy.context.object
    light.name = "cyber_softbox_key_light"
    light.data.energy = 650
    light.data.size = 5.0

    bpy.ops.object.camera_add(location=(0, -8, 2.2), rotation=(math.radians(78), 0, 0))
    camera = bpy.context.object
    camera.name = "orthographic_side_camera"
    camera.data.type = "ORTHO"
    camera.data.ortho_scale = 4.0
    bpy.context.scene.camera = camera


def main() -> None:
    args = parse_args()
    out_dir = Path(args.out)
    out_dir.mkdir(parents=True, exist_ok=True)

    clear_scene()
    setup_scene()
    mats = create_materials()

    roots = [
        (build_guardian(mats), "cg_guardian_player.glb"),
        (build_virus(mats), "cg_virus_grunt.glb"),
        (build_boss(mats), "cg_malware_boss.glb"),
        (build_quiz_blocks(mats), "cg_quiz_shield_blocks.glb"),
        (build_platform_traps(mats), "cg_platform_traps.glb"),
        (build_projectiles(mats), "cg_projectiles.glb"),
    ]

    blend_path = out_dir / "cyber_guardian_asset_factory_source.blend"
    bpy.ops.wm.save_as_mainfile(filepath=str(blend_path))

    if not args.no_glb:
        for root, filename in roots:
            export_root(root, out_dir, filename)

    if not args.no_render:
        render_sprite_set(roots, out_dir)

    print(f"Cyber Guardian assets written to: {out_dir}")


if __name__ == "__main__":
    main()
