import React, { useState, useEffect } from 'react';
import './App.css';

const API_BASE_URL = 'http://localhost:5000/api';

interface TreasureResult {
  minFuel: number;
}

interface HistoryItem {
  id: number;
  n: number;
  m: number;
  p: number;
  matrix: string[][];
  result: number;
  createdAt: string;
}

export default function TreasureHuntApp() {
  const [n, setN] = useState('3');
  const [m, setM] = useState('3');
  const [p, setP] = useState('3');
  const [matrix, setMatrix] = useState<string[][]>([]);
  const [result, setResult] = useState<TreasureResult | null>(null);
  const [history, setHistory] = useState<HistoryItem[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    initializeMatrix();
    loadHistory();
  }, [n, m]);

  const initializeMatrix = () => {
    const rows = parseInt(n) || 0;
    const cols = parseInt(m) || 0;
    if (isNaN(rows) || isNaN(cols) || rows < 1 || rows > 500 || cols < 1 || cols > 500) {
      setMatrix([]);
      return;
    }
    const newMatrix = Array(rows).fill(null).map(() => Array(cols).fill('1'));
    setMatrix(newMatrix);
  };

  const loadHistory = async () => {
    try {
      const response = await fetch(`${API_BASE_URL}/treasurehunt/history`);
      if (response.ok) {
        const data = await response.json();
        setHistory(data);
      }
    } catch (err) {
      console.error('Error loading history:', err);
    }
  };

  const handleMatrixChange = (i: number, j: number, value: string) => {
    const newMatrix = [...matrix];
    newMatrix[i][j] = value;
    setMatrix(newMatrix);
  };

  const validateInput = () => {
    const rows = parseInt(n);
    const cols = parseInt(m);
    const maxP = parseInt(p);

    if (rows < 1 || rows > 500) {
      setError('Số hàng phải từ 1 đến 500');
      return false;
    }
    if (cols < 1 || cols > 500) {
      setError('Số cột phải từ 1 đến 500');
      return false;
    }
    if (maxP < 1 || maxP > rows * cols) {
      setError(`p phải từ 1 đến ${rows * cols}`);
      return false;
    }

    for (let i = 0; i < rows; i++) {
      for (let j = 0; j < cols; j++) {
        const val = parseInt(matrix[i][j]);
        if (isNaN(val) || val < 1 || val > maxP) {
          setError(`Giá trị tại [${i + 1},${j + 1}] phải từ 1 đến ${maxP}`);
          return false;
        }
      }
    }

    const matrixFlat = matrix.flat().map(v => parseInt(v));
    if (!matrixFlat.includes(maxP)) {
      setError(`Ma trận phải chứa ít nhất một rương số ${maxP}`);
      return false;
    }

    setError('');
    return true;
  };

  const handleSolve = async () => {
    if (!validateInput()) return;

    setLoading(true);
    setResult(null);

    try {
      const response = await fetch(`${API_BASE_URL}/treasurehunt/solve`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          n: parseInt(n),
          m: parseInt(m),
          p: parseInt(p),
          matrix: matrix.map(row => row.map(v => parseInt(v)))
        })
      });

      if (response.ok) {
        const data = await response.json();
        setResult(data);
        loadHistory();
      } else {
        const errorData = await response.json();
        setError(errorData.message || 'Có lỗi xảy ra');
      }
    } catch (err) {
      setError('Không thể kết nối đến server');
    } finally {
      setLoading(false);
    }
  };

  const loadFromHistory = (item: HistoryItem) => {
    setN(item.n.toString());
    setM(item.m.toString());
    setP(item.p.toString());
    setMatrix(item.matrix);
    setResult({ minFuel: item.result });
  };

  return (
    <div className="container">
      <h1>Tìm Kho Báu</h1>

      <div className="main-grid">
        <div>
          <div className="paper">
            <h2>Nhập thông tin bản đồ</h2>

            <div className="input-row">
              <div className="input-group">
                <label>Số hàng (n)</label>
                <input
                  type="number"
                  value={n}
                  onChange={(e) => setN(e.target.value)}
                  min="1"
                  max="500"
                />
              </div>
              <div className="input-group">
                <label>Số cột (m)</label>
                <input
                  type="number"
                  value={m}
                  onChange={(e) => setM(e.target.value)}
                  min="1"
                  max="500"
                />
              </div>
              <div className="input-group">
                <label>Số loại rương (p)</label>
                <input
                  type="number"
                  value={p}
                  onChange={(e) => setP(e.target.value)}
                  min="1"
                  max={parseInt(n) * parseInt(m)}
                />
              </div>
            </div>

            <label>Ma trận kho báu:</label>
            <div className="matrix-container">
              <table>
                <thead>
                  <tr>
                    <th></th>
                    {Array.from({ length: parseInt(m) || 0 }).map((_, j) => (
                      <th key={j}>{j + 1}</th>
                    ))}
                  </tr>
                </thead>
                <tbody>
                  {matrix.map((row, i) => (
                    <tr key={i}>
                      <th>{i + 1}</th>
                      {row.map((cell, j) => (
                        <td key={j}>
                          <input
                            className="matrix-input"
                            type="number"
                            value={cell}
                            onChange={(e) => handleMatrixChange(i, j, e.target.value)}
                            min="1"
                            max={parseInt(p)}
                          />
                        </td>
                      ))}
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>

            {error && <div className="alert">{error}</div>}

            <button
              className="btn btn-primary"
              onClick={handleSolve}
              disabled={loading}
            >
              {loading ? <><span className="spinner"></span> Đang tính toán...</> : 'Giải bài toán'}
            </button>

            {result && (
              <div className="result-card">
                <div>Nhiên liệu tối thiểu</div>
                <h3>{result.minFuel.toFixed(5)}</h3>
              </div>
            )}
          </div>

          <div className="description">
            <h2>Mô tả bài toán</h2>
            <p>
              Đoàn hải tặc cần tìm đường đi từ vị trí (1,1) để thu thập các chìa khóa theo thứ tự từ 0 đến p
              và cuối cùng đến rương kho báu đánh số p.
            </p>
            <p>
              Nhiên liệu cần thiết để di chuyển từ (x1, y1) đến (x2, y2) là: √[(x1-x2)² + (y1-y2)²]
            </p>
            <p>
              <strong>Mục tiêu:</strong> Tìm lộ trình tiêu tốn ít nhiên liệu nhất.
            </p>
          </div>
        </div>

        <div className="paper">
          <h2>Lịch sử giải bài</h2>

          {history.length === 0 ? (
            <div className="no-history">Chưa có lịch sử</div>
          ) : (
            <div style={{ maxHeight: '700px', overflow: 'auto' }}>
              {history.map((item) => (
                <div
                  key={item.id}
                  className="history-item"
                  onClick={() => loadFromHistory(item)}
                >
                  <div className="history-date">
                    {new Date(item.createdAt).toLocaleString('vi-VN')}
                  </div>
                  <div className="history-info">
                    Ma trận: {item.n} × {item.m}, p = {item.p}
                  </div>
                  <div className="history-result">
                    Kết quả: {item.result.toFixed(5)}
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>
    </div>
  );
}