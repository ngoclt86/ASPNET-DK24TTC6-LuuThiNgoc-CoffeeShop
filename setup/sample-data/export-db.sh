#!/usr/bin/env bash
# Xuất database SQL Server (schema + dữ liệu) ra file SQL, dùng SqlPackage (Extract -> Script).
# Yêu cầu: SQL Server đang chạy và database đã tồn tại (đã migrate / đã chạy app).

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
TOOLS_DIR="${SCRIPT_DIR}/.tools"
DACPAC_TMP="${SCRIPT_DIR}/_export_temp.dacpac"
OUT_SQL="${SCRIPT_DIR}/exported-database.sql"

# Mật khẩu mặc định trùng docker/compose.yaml (tránh dùng ${VAR:-pass##} vì ## bị hiểu nhầm trong bash)
DEFAULT_SQL_PASSWORD='1234qwer##'
SQL_SERVER="${SQL_SERVER:-127.0.0.1}"
SQL_PORT="${SQL_PORT:-1433}"
SQL_DATABASE="${SQL_DATABASE:-CafeShopDb}"
SQL_USER="${SQL_USER:-sa}"
SQL_PASSWORD="${SQL_PASSWORD:-$DEFAULT_SQL_PASSWORD}"

mkdir -p "${TOOLS_DIR}"

detect_sqlpackage() {
  if command -v sqlpackage >/dev/null 2>&1; then
    echo "sqlpackage"
    return
  fi
  local cached="${TOOLS_DIR}/sqlpackage"
  if [[ -x "${cached}" ]]; then
    echo "${cached}"
    return
  fi
  echo ""
}

download_sqlpackage() {
  local os arch zip dest
  os="$(uname -s)"
  arch="$(uname -m)"
  zip="${TOOLS_DIR}/sqlpackage-download.zip"
  dest="${TOOLS_DIR}/sqlpackage"

  case "${os}-${arch}" in
    Darwin-arm64|Darwin-x86_64)
      echo "Đang tải SqlPackage (macOS)..." >&2
      curl -fsSL "https://aka.ms/sqlpackage-macos" -o "${zip}"
      ;;
    Linux-x86_64)
      echo "Đang tải SqlPackage (Linux x64)..." >&2
      curl -fsSL "https://aka.ms/sqlpackage-linux" -o "${zip}"
      ;;
    Linux-aarch64|Linux-arm64)
      echo "Đang tải SqlPackage (Linux ARM)..." >&2
      curl -fsSL "https://aka.ms/sqlpackage-linux-net6.0" -o "${zip}" || {
        echo "Không tìm thấy bản SqlPackage phù hợp cho ${os}-${arch}. Cài sqlpackage (brew install sqlpackage) hoặc dùng SSMS để Generate Scripts." >&2
        exit 1
      }
      ;;
    *)
      echo "Hệ điều hành/kiến trúc không được hỗ trợ tự động: ${os}-${arch}. Cài SqlPackage: https://learn.microsoft.com/sql/tools/sqlpackage/sqlpackage-download" >&2
      exit 1
      ;;
  esac

  rm -rf "${TOOLS_DIR}/sqlpackage_extract"
  mkdir -p "${TOOLS_DIR}/sqlpackage_extract"
  unzip -q -o "${zip}" -d "${TOOLS_DIR}/sqlpackage_extract"
  rm -f "${zip}"

  local bin
  bin="$(find "${TOOLS_DIR}/sqlpackage_extract" -type f -name sqlpackage 2>/dev/null | head -1)"
  if [[ -z "${bin}" ]]; then
    echo "Không tìm thấy file thực thi sqlpackage trong zip." >&2
    exit 1
  fi
  chmod +x "${bin}"
  mv "${bin}" "${dest}"
  rm -rf "${TOOLS_DIR}/sqlpackage_extract"
  echo "${dest}"
}

resolve_sqlpackage() {
  local found
  found="$(detect_sqlpackage)"
  if [[ -n "${found}" ]]; then
    echo "${found}"
    return
  fi
  download_sqlpackage
}

CONN="Server=${SQL_SERVER},${SQL_PORT};Initial Catalog=${SQL_DATABASE};User ID=${SQL_USER};Password=${SQL_PASSWORD};Encrypt=False;TrustServerCertificate=True;MultipleActiveResultSets=False"

echo "Kết nối: Server=${SQL_SERVER},${SQL_PORT}; Database=${SQL_DATABASE}; User=${SQL_USER}"
SQLPACKAGE_BIN="$(resolve_sqlpackage)"
echo "SqlPackage: ${SQLPACKAGE_BIN}"
echo "Bước 1/2: Extract -> ${DACPAC_TMP}"

rm -f "${DACPAC_TMP}"
"${SQLPACKAGE_BIN}" /Action:Extract \
  /TargetFile:"${DACPAC_TMP}" \
  /SourceConnectionString:"${CONN}" \
  /p:ExtractAllTableData=True

echo "Bước 2/2: Script -> ${OUT_SQL}"
rm -f "${OUT_SQL}"
"${SQLPACKAGE_BIN}" /Action:Script \
  /SourceFile:"${DACPAC_TMP}" \
  /TargetFile:"${OUT_SQL}" \
  /p:ScriptForTheDatabaseEngineVersion=True

rm -f "${DACPAC_TMP}"
echo "Hoàn tất: ${OUT_SQL}"
